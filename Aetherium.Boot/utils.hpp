//
//  utils.hpp
//  Aetherium.Boot
//
//  Created by Marc-Aurel Zent on 11.06.23.
//

#ifndef utils_hpp
#define utils_hpp

#include <filesystem>
#include <mach/arm/vm_types.h>

namespace utils {
    std::filesystem::path get_executable_path();
    std::filesystem::path get_application_support_path();
    uint64_t get_base_adress();
    void showAlert(const char *message, const char *info, const char *buttonTitle);
}

#endif /* utils_hpp */
