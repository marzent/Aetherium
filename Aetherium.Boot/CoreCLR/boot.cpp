#include <cstdio>
#include <stdlib.h>
#include <filesystem>
#include <iostream>
#include <optional>

#include "CoreCLR.hpp"
#include "logging.hpp"

std::optional<CoreCLR> g_clr;

int InitializeClrAndGetEntryPoint(
    std::string calling_module_path,
    std::string runtimeconfig_path,
    std::string module_path,
    std::string entrypoint_assembly_name,
    std::string entrypoint_method_name,
    void** entrypoint_fn)
{
    g_clr.emplace();

    int result;
    setenv("DOTNET_MULTILEVEL_LOOKUP", "0", 1);
    setenv("COMPlus_legacyCorruptedStateExceptionsPolicy", "1", 1);
    setenv("DOTNET_legacyCorruptedStateExceptionsPolicy", "1", 1);
    setenv("COMPLUS_ForceENC", "1", 1);
    setenv("DOTNET_ForceENC", "1", 1);

    // Enable Dynamic PGO
    setenv("DOTNET_TieredPGO", "1", 1);
    setenv("DOTNET_TC_QuickJitForLoops", "1", 1);
    setenv("DOTNET_ReadyToRun", "1", 1);

    char_t* dotnet_path;

    std::string buffer;
    buffer.resize(0);
    dotnet_path = getenv("AETHERIUM_RUNTIME");
    if (!dotnet_path || !std::filesystem::exists(dotnet_path))
    {
        dotnet_path = (char_t*)"/usr/local/share/dotnet";
    }

    // =========================================================================== //

    logging::I("with dotnet_path: {}", dotnet_path);
    logging::I("with config_path: {}", runtimeconfig_path);
    logging::I("with calling_module_path: {}", calling_module_path);
    logging::I("with module_path: {}", module_path);

    if (!std::filesystem::exists(dotnet_path))
    {
        logging::E("Error: Unable to find .NET runtime path");
        return 1;
    }

    get_hostfxr_parameters init_parameters
    {
        sizeof(get_hostfxr_parameters),
        nullptr,
        dotnet_path,
    };

    logging::I("Loading hostfxr...");
    if ((result = g_clr->load_hostfxr(&init_parameters)) != 0)
    {
        logging::E("Failed to load the `hostfxr` library (err=0x{:08x})", result);
        return result;
    }
    logging::I("Done!");

    // =========================================================================== //

    hostfxr_initialize_parameters runtime_parameters
    {
        sizeof(hostfxr_initialize_parameters),
        module_path.c_str(),
        dotnet_path,
    };

    logging::I("Loading coreclr... ");
    if ((result = g_clr->load_runtime(runtimeconfig_path, &runtime_parameters)) != 0)
    {
        logging::E("Failed to load coreclr (err=0x{:08X})", static_cast<uint32_t>(result));
        return result;
    }
    logging::I("Done!");

    // =========================================================================== //

    logging::I("Loading module from {}...", module_path.c_str());
    if ((result = g_clr->load_assembly_and_get_function_pointer(
        module_path.c_str(),
        entrypoint_assembly_name.c_str(),
        entrypoint_method_name.c_str(),
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr, entrypoint_fn)) != 0)
    {
        logging::E("Failed to load module (err={})", result);
        return result;
    }
    logging::I("Done!");

    // =========================================================================== //

    return 0;
}
