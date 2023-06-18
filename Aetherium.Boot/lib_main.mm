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

}
