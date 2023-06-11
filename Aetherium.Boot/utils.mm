//
//  utils.mm
//  Aetherium.Boot
//
//  Created by Marc-Aurel Zent on 11.06.23.
//

#include <mach-o/dyld.h>
#import <Foundation/Foundation.h>

#include "utils.hpp"

std::filesystem::path utils::get_executable_path() {
    char buf [PATH_MAX];
    unsigned int bufsize = PATH_MAX;
    if(!_NSGetExecutablePath(buf, &bufsize))
        return std::filesystem::path(buf);
    return std::filesystem::path();
}

std::filesystem::path utils::get_application_support_path() {
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSApplicationSupportDirectory, NSUserDomainMask, YES);
    NSString *applicationSupportDirectory = [paths firstObject];
    return std::filesystem::path([applicationSupportDirectory UTF8String]);
}
