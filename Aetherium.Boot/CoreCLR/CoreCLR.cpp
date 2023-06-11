#include <filesystem>
#include <iostream>
#include <dlfcn.h>

#include "CoreCLR.hpp"
#include "logging.hpp"
#include "nethost/nethost.h"

/* Core public functions */
int CoreCLR::load_hostfxr()
{
    return CoreCLR::load_hostfxr(nullptr);
}

int CoreCLR::load_hostfxr(const struct get_hostfxr_parameters* parameters)
{
    // Get the path to CoreCLR's hostfxr
    char_t buffer[PATH_MAX]{};
    size_t buffer_size = sizeof buffer / sizeof(char_t);
    if (int rc = get_hostfxr_path(buffer, &buffer_size, parameters); rc != 0)
        return rc;

    logging::D("Resolved hostfxr path: {}", buffer);

    // Load hostfxr and get desired exports
    auto lib_hostfxr = reinterpret_cast<void*>(load_library(buffer));
    if (!lib_hostfxr)
        return -1;

    m_hostfxr_initialize_for_runtime_config_fptr = reinterpret_cast<hostfxr_initialize_for_runtime_config_fn>(
        get_export(lib_hostfxr, "hostfxr_initialize_for_runtime_config"));
    m_hostfxr_get_runtime_delegate_fptr = reinterpret_cast<hostfxr_get_runtime_delegate_fn>(
        get_export(lib_hostfxr, "hostfxr_get_runtime_delegate"));
    m_hostfxr_close_fptr = reinterpret_cast<hostfxr_close_fn>(
        get_export(lib_hostfxr, "hostfxr_close"));

    return m_hostfxr_initialize_for_runtime_config_fptr
        && m_hostfxr_get_runtime_delegate_fptr
        && m_hostfxr_close_fptr ? 0 : -1;
}

int CoreCLR::load_runtime(const std::string& runtime_config_path)
{
    return CoreCLR::load_runtime(runtime_config_path, nullptr);
}

int CoreCLR::load_runtime(const std::string& runtime_config_path, const struct hostfxr_initialize_parameters* parameters)
{
    int result;

    // Load .NET Core
    hostfxr_handle context = nullptr;
    result = m_hostfxr_initialize_for_runtime_config_fptr(
        runtime_config_path.c_str(),
        parameters,
        &context);
    
    // Success_HostAlreadyInitialized
    if (result == 1)
    {
        logging::I("Success_HostAlreadyInitialized (0x1)");
        result = 0;
    }

    if (result != 0 || context == nullptr)
    {
        m_hostfxr_close_fptr(context);
        return result;
    }

    // Get the load assembly function pointer
    result = m_hostfxr_get_runtime_delegate_fptr(
        context,
        hdt_load_assembly_and_get_function_pointer,
        reinterpret_cast<void**>(&m_load_assembly_and_get_function_pointer_fptr));

    if (result != 0 || m_load_assembly_and_get_function_pointer_fptr == nullptr) {
        m_hostfxr_close_fptr(context);
        return result;
    }

    result = m_hostfxr_get_runtime_delegate_fptr(
        context,
        hdt_get_function_pointer,
        reinterpret_cast<void**>(&m_get_function_pointer_fptr));

    if (result != 0 || m_get_function_pointer_fptr == nullptr)
    {
        m_hostfxr_close_fptr(context);
        return result;
    }

    m_hostfxr_close_fptr(context);

    return 0;
}

int CoreCLR::load_assembly_and_get_function_pointer(
    const char_t* assembly_path,
    const char_t* type_name,
    const char_t* method_name,
    const char_t* delegate_type_name,
    void* reserved,
    void** delegate) const
{
    int result = m_load_assembly_and_get_function_pointer_fptr(assembly_path, type_name, method_name, delegate_type_name, reserved, delegate);

    if (result != 0)
        delegate = nullptr;

    return result;
};

int CoreCLR::get_function_pointer(
    const char_t* type_name,
    const char_t* method_name,
    const char_t* delegate_type_name,
    void* load_context,
    void* reserved,
    void** delegate) const
{
    int result = m_get_function_pointer_fptr(type_name, method_name, delegate_type_name, load_context, reserved, delegate);

    if (result != 0)
        delegate = nullptr;

    return result;
}

/* Helpers */
void* CoreCLR::load_library(const char_t* path)
{
    void *h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
    assert(h != nullptr);
    return h;
}

void* CoreCLR::get_export(void* h, const char* name)
{
    void *f = dlsym(h, name);
    assert(f != nullptr);
    return f;
}
