//
//  lib_main.cpp
//  Aetherium.Injector
//
//  Created by Marc-Aurel Zent on 11.06.23.
//

#include "initialize.hpp"
#include "utils.hpp"

__attribute__((constructor))
int lib_main(int argc, char **argv) {
    return initialize(argc, argv);
}
