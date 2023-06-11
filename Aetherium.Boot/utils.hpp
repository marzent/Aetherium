//
//  utils.hpp
//  Aetherium.Boot
//
//  Created by Marc-Aurel Zent on 11.06.23.
//

#ifndef utils_hpp
#define utils_hpp

#include <filesystem>

namespace utils {
    std::filesystem::path get_executable_path();
    std::filesystem::path get_application_support_path();
}

#endif /* utils_hpp */
