#ifndef boot_hpp
#define boot_hpp

int InitializeClrAndGetEntryPoint(
    std::string calling_module_path,
    std::string runtimeconfig_path,
    std::string module_path,
    std::string entrypoint_assembly_name,
    std::string entrypoint_method_name,
    void** entrypoint_fn);

#endif /* boot_hpp */
