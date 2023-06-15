//
//  utils.mm
//  Aetherium.Boot
//
//  Created by Marc-Aurel Zent on 11.06.23.
//

#include <mach-o/dyld.h>
#include <mach/mach_init.h>
#include <mach/mach_vm.h>
#include <sys/sysctl.h>
#import <Cocoa/Cocoa.h>

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

vm_map_offset_t utils::get_base_adress() {
    mach_port_name_t task = current_task();
    vm_map_offset_t vmoffset;
    vm_map_size_t vmsize;
    uint32_t nesting_depth = 0;
    struct vm_region_submap_info_64 vbr;
    mach_msg_type_number_t vbrcount = 16;
    kern_return_t kr;

    if ((kr = mach_vm_region_recurse(task, &vmoffset, &vmsize, &nesting_depth, (vm_region_recurse_info_t)&vbr, &vbrcount)) != KERN_SUCCESS)
        return 0;

    return vmoffset;
}

void utils::showAlert(const char *message, const char *info, const char *buttonTitle) {
    if (strcmp(dispatch_queue_get_label(DISPATCH_CURRENT_QUEUE_LABEL), dispatch_queue_get_label(dispatch_get_main_queue())) == 0) {
        NSAlert *alert = [[NSAlert alloc] init];
        if (message)
            [alert setMessageText:[NSString stringWithUTF8String:message]];
        if (info)
            [alert setInformativeText:[NSString stringWithUTF8String:info]];
        if (buttonTitle)
            [alert addButtonWithTitle:[NSString stringWithUTF8String:buttonTitle]];
        [alert runModal];
    } else {
        dispatch_sync(dispatch_get_main_queue(), ^{
            showAlert(message, info, buttonTitle);
        });
    }
}
