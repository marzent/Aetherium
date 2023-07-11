//
//  initialize.cpp
//  Aetherium.Boot
//
//  Created by Marc-Aurel Zent on 11.06.23.
//

#include "initialize.hpp"
#include "utils.hpp"
#include "logging.hpp"
#include "CoreCLR/boot.hpp"
#include "CoreCLR/core/coreclr_delegates.h"

int initialize(int argc, char **argv) {
    const auto executable_path = utils::get_executable_path();
    const auto aetherium_path = utils::get_application_support_path() / "Aetherium";
    const auto logFilePath = aetherium_path / "aetherium.boot.log";

    if (logFilePath.empty()) {
        logging::I("No log file path given; not logging to file.");
    } else {
        try {
            logging::start_file_logging(logFilePath, false);
            logging::I("Logging to file: {}", logFilePath);
            
        } catch (const std::invalid_argument& e) {
            logging::E("Couldn't open log file: {}", logFilePath);
            logging::E("Error: {} / {}", errno, e.what());
        }
    }

    logging::I("Aetherium.Boot Injectable, (c) 2023 marzent (c) 2021 XIVLauncher Contributors");
    logging::I("Built at: " __DATE__ "@" __TIME__);
    
    const auto base_adress = utils::get_base_adress();
    
    logging::I("Executing {} with base adress {:#x}", executable_path, base_adress);

    const auto runtimeconfig_path = (aetherium_path / "bin" / "Aetherium.runtimeconfig.json").string();
    const auto module_path = (aetherium_path / "bin" / "Aetherium.dll").string();

    // ============================== CLR ========================================= //

    logging::I("Calling InitializeClrAndGetEntryPoint");

    void* entrypoint_vfn;
    int result = InitializeClrAndGetEntryPoint(
        executable_path,
        runtimeconfig_path,
        module_path,
        "Aetherium.EntryPoint, Aetherium",
        "Initialize",
        &entrypoint_vfn);

    if (result != 0)
        return result;

    using custom_component_entry_point_fn = void (CORECLR_DELEGATE_CALLTYPE*)(const char*, uint64_t);
    const auto entrypoint_fn = reinterpret_cast<custom_component_entry_point_fn>(entrypoint_vfn);

    // ============================== Aetherium ==================================== //

    logging::I("Initializing Aetherium...");
    entrypoint_fn(executable_path.c_str(), base_adress);

    logging::I("Done!");

    return 0;
}

