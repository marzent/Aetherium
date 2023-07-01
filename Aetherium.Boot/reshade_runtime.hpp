//
//  reshade_runtime.hpp
//  Aetherium.Boot
//
//  Created by Marc-Aurel Zent on 28.06.23.
//

#ifndef reshade_runtime_hpp
#define reshade_runtime_hpp

#define RESHADE_VERSION_MAJOR 4
#define RESHADE_VERSION_MINOR 8
#define RESHADE_VERSION_REVISION 0

#include "c_effect.h"

namespace reshade_runtime {
    struct module *load_effect(const std::filesystem::path &source_file, int effect_width, int effect_height);
    char *spirv_to_msl(char *spirv_data, long spirv_size, const char *entry_point, shader_type type);
}

#endif /* reshade_runtime_hpp */
