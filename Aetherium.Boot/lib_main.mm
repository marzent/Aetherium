//
//  lib_main.cpp
//  Aetherium.Injector
//
//  Created by Marc-Aurel Zent on 11.06.23.
//

#import <Cocoa/Cocoa.h>
#import <Metal/Metal.h>

#include "initialize.hpp"
#include "utils.hpp"
#include "imgui_impl_osx.h"
#include "imgui_impl_metal.h"
#include "reshade_runtime.hpp"
#include "logging.hpp"

__attribute__((constructor))
int lib_main(int argc, char **argv) {
    return initialize(argc, argv);
}

extern "C" {

__attribute__((visibility("default")))
void showAlert(const char *message, const char *info, const char *buttonTitle) {
    utils::showAlert(message, info, buttonTitle);
}

__attribute__((visibility("default")))
NSWindow *getMainWindow() {
    __block NSWindow *mainWindow = nil;

    if ([NSThread isMainThread]) {
        mainWindow = [[[NSApplication sharedApplication] windows] firstObject];
    } else {
        dispatch_sync(dispatch_get_main_queue(), ^{
            mainWindow = [[[NSApplication sharedApplication] windows] firstObject];
        });
    }

    if (mainWindow != nil)
        return mainWindow;

    [NSThread sleepForTimeInterval:0.001];
    return getMainWindow();
}

__attribute__((visibility("default")))
bool ImGui_ImplMetal_Init(void* device){
    return ImGui_ImplMetal_Init((__bridge id<MTLDevice>) device);
}

__attribute__((visibility("default")))
void ImGui_ImplMetal_NewFrame(void* renderPassDescriptor){
    ImGui_ImplMetal_NewFrame((__bridge MTLRenderPassDescriptor*) renderPassDescriptor);
}

__attribute__((visibility("default")))
void ImGui_ImplMetal_RenderDrawData(ImDrawData* drawData, void* commandBuffer, void* commandEncoder){
    ImGui_ImplMetal_RenderDrawData(drawData, (__bridge id<MTLCommandBuffer>) commandBuffer, (__bridge id<MTLRenderCommandEncoder> )commandEncoder);
}

__attribute__((visibility("default")))
void ImGui_ImplMetal_DeInit(void){
    ImGui_ImplMetal_Shutdown();
}

__attribute__((visibility("default")))
bool ImGui_ImplMacOS_Init(void* view) {
    return ImGui_ImplOSX_Init((__bridge NSView*)view);
}

__attribute__((visibility("default")))
void ImGui_ImplMacOS_DeInit(void){
    ImGui_ImplOSX_Shutdown();
}

__attribute__((visibility("default")))
void ImGui_ImplMacOS_NewFrame(void* view){
    ImGui_ImplOSX_NewFrame((__bridge NSView*)view);
}

__attribute__((visibility("default")))
void ImGui_ImplMacOS_NewViewportFrame(){
    dispatch_async(dispatch_get_main_queue(), ^{
        if (ImGui::GetIO().ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
        {
            ImGui::UpdatePlatformWindows();
            ImGui::RenderPlatformWindowsDefault();
        }
    });
}

__attribute__((visibility("default")))
struct module *reShadeLoadEffect(const char *source_file, int effect_width, int effect_height) {
    try {
        return reshade_runtime::load_effect(std::filesystem::path(source_file), effect_width, effect_height);
    }
    catch (...) {
        logging::E("An error loading the effect file {} has occured", source_file);
        return NULL;
    }
}

__attribute__((visibility("default")))
void freeModule(struct module *module) {
    free_module(module);
}

__attribute__((visibility("default")))
char *spirvToMsl(char *spirv_data, long spirv_size, const char *entry_point, shader_type type) {
    try {
        return reshade_runtime::spirv_to_msl(spirv_data, spirv_size, entry_point, type);
    }
    catch (const std::exception& e) {
        logging::E("Error: {} / {}", errno, e.what());
        return NULL;
    }
}

}
