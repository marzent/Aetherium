//
//  reshade_runtime.cpp
//  Aetherium.Boot
//
//  Created by Marc-Aurel Zent on 28.06.23.
//

#include <iostream>
#include <fstream>
#include <set>
#include "reshade_runtime.hpp"
#include "effect_parser.hpp"
#include "effect_codegen.hpp"
#include "effect_preprocessor.hpp"
#include "spirv_msl.hpp"
#include "logging.hpp"

static bool resolve_path(std::filesystem::path &path, std::error_code &ec)
{
    // Path must be absolute (for us it always will be)
    if (std::filesystem::path canonical_path = std::filesystem::canonical(path, ec); !ec)
        path = std::move(canonical_path);
    return !ec; // The canonicalization step fails if the path does not exist
}

struct module *reshade_runtime::load_effect(const std::filesystem::path &source_file, int effect_width, int effect_height)
{
    const std::string effect_name = source_file.filename().string();
    
    logging::D("Loading effect {} from file {}", effect_name, source_file.string());
    
    std::error_code ec;
    std::set<std::filesystem::path> include_paths;
    if (source_file.is_absolute())
        include_paths.emplace(source_file.parent_path());
    for (std::filesystem::path include_path : include_paths)
    {
        if (resolve_path(include_path, ec))
        {
            include_paths.emplace(include_path);

            for (const std::filesystem::directory_entry &entry : std::filesystem::recursive_directory_iterator(include_path, std::filesystem::directory_options::skip_permission_denied, ec))
                if (entry.is_directory(ec))
                    include_paths.emplace(entry);
        }
    }

    bool skip_optimization = false;
    std::string code_preamble;
    std::string source;
    
    reshadefx::preprocessor pp;
    pp.add_macro_definition("__RESHADE__", std::to_string(RESHADE_VERSION_MAJOR * 10000 + RESHADE_VERSION_MINOR * 100 + RESHADE_VERSION_REVISION));
    pp.add_macro_definition("__RESHADE_PERFORMANCE_MODE__", "1");
    pp.add_macro_definition("__VENDOR__", "APPLE");
    pp.add_macro_definition("__DEVICE__", "APPLE_SILICON");
    pp.add_macro_definition("__RENDERER__", "METAL");
    pp.add_macro_definition("__APPLICATION__", "AETHERIUM");
    pp.add_macro_definition("BUFFER_WIDTH", std::to_string(effect_width));
    pp.add_macro_definition("BUFFER_HEIGHT", std::to_string(effect_height));
    pp.add_macro_definition("BUFFER_RCP_WIDTH", "(1.0 / BUFFER_WIDTH)");
    pp.add_macro_definition("BUFFER_RCP_HEIGHT", "(1.0 / BUFFER_HEIGHT)");
    //pp.add_macro_definition("BUFFER_COLOR_SPACE", std::to_string(static_cast<uint32_t>(_back_buffer_color_space)));
    //pp.add_macro_definition("BUFFER_COLOR_BIT_DEPTH", std::to_string(format_color_bit_depth(_effect_color_format)));

    for (const std::filesystem::path &include_path : include_paths)
        pp.add_include_path(include_path);

    // Add some conversion macros for compatibility with older versions of ReShade
    pp.append_string(
        "#define tex2Doffset(s, coords, offset) tex2D(s, coords, offset)\n"
        "#define tex2Dlodoffset(s, coords, offset) tex2Dlod(s, coords, offset)\n"
        "#define tex2Dgather(s, t, c) tex2Dgather##c(s, t)\n"
        "#define tex2Dgatheroffset(s, t, o, c) tex2Dgather##c(s, t, o)\n"
        "#define tex2Dgather0 tex2DgatherR\n"
        "#define tex2Dgather1 tex2DgatherG\n"
        "#define tex2Dgather2 tex2DgatherB\n"
        "#define tex2Dgather3 tex2DgatherA\n");

    // Load and preprocess the source file
    bool preprocessed = pp.append_file(source_file);
    
    if (!preprocessed) {
        logging::E("{}", pp.errors());
        return NULL;
    }

    source = pp.output();

    for (const std::pair<std::string, std::string> &pragma : pp.used_pragma_directives())
    {
        if (pragma.first == "reshade")
        {
            if (pragma.second == "skipoptimization" || pragma.second == "nooptimization")
                skip_optimization = true;
            continue;
        }

        const std::string pragma_directive = "#pragma " + pragma.first + ' ' + pragma.second + '\n';

        code_preamble += pragma_directive;
        source = "// " + pragma_directive + source;
    }

    if (source.empty()) return NULL;

    std::unique_ptr<reshadefx::codegen> codegen;
    codegen.reset(reshadefx::create_codegen_spirv(true, true, true, false, false));

    reshadefx::parser parser;

    bool compiled = parser.parse(std::move(source), codegen.get());
    
    if (!compiled) {
        logging::E("{}", parser.errors());
        return NULL;
    }

    reshadefx::module module;
    codegen->write_result(module);
    struct module *c_module = build_module(module);
    
    if (!c_module)
        logging::E("Conversion to C module failed.");
    
    return c_module;
}

static spv::ExecutionModel ConvertExecutionModel(shader_type model)
{
    switch (model)
    {
        case shader_type_vs:
            return spv::ExecutionModelVertex;
        case shader_type_ps:
            return spv::ExecutionModelFragment;
        case shader_type_cs:
            return spv::ExecutionModelGLCompute;
    }
}

char *reshade_runtime::spirv_to_msl(char *_spirv_data, long _spirv_size, const char *entry_point, shader_type type) {
    const uint32_t* spirv_data = reinterpret_cast<const uint32_t*>(_spirv_data);
    size_t spirvSize = _spirv_size / sizeof(uint32_t);
    
    spirv_cross::CompilerMSL compiler(spirv_data, spirvSize);
    spirv_cross::CompilerMSL::Options options;
    options.msl_version = spirv_cross::CompilerMSL::Options::make_msl_version(2, 3);
    compiler.set_msl_options(options);
    compiler.set_entry_point(entry_point, ConvertExecutionModel(type));
    
    std::string msl_shader_code = compiler.compile();
    return strdup(msl_shader_code.c_str());
}
