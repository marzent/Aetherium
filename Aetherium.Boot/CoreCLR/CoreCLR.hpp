#ifndef CoreCLR_hpp
#define CoreCLR_hpp

#include <string>
#include "core/hostfxr.h"
#include "core/coreclr_delegates.h"
#include "nethost/nethost.h"

class CoreCLR {
public:
    explicit CoreCLR() {};
    ~CoreCLR() = default;

    int load_hostfxr();
    int load_hostfxr(const get_hostfxr_parameters* parameters);

    int load_runtime(const std::string& runtime_config_path);
    int load_runtime(
        const std::string& runtime_config_path,
        const struct hostfxr_initialize_parameters* parameters);

    int load_assembly_and_get_function_pointer(
        const char_t* assembly_path,
        const char_t* type_name,
        const char_t* method_name,
        const char_t* delegate_type_name,
        void* reserved,
        void** delegate) const;
    int get_function_pointer(
        const char_t* type_name,
        const char_t* method_name,
        const char_t* delegate_type_name,
        void* load_context,
        void* reserved,
        void** delegate) const;

private:
    /* HostFXR delegates. */
    hostfxr_initialize_for_runtime_config_fn m_hostfxr_initialize_for_runtime_config_fptr{};
    hostfxr_get_runtime_delegate_fn m_hostfxr_get_runtime_delegate_fptr{};
    hostfxr_close_fn m_hostfxr_close_fptr{};
    get_function_pointer_fn m_get_function_pointer_fptr = nullptr;
    load_assembly_and_get_function_pointer_fn m_load_assembly_and_get_function_pointer_fptr = nullptr;

    /* Helper functions. */
    static void* load_library(const char_t* path);
    static void* get_export(void* h, const char* name);
};

#endif /* CoreCLR_hpp */
