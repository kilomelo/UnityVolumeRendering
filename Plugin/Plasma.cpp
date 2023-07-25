#pragma once
#include <math.h>
#include <stdint.h>
#include <stdlib.h>
#include <string>
#include <cstring>
#include "Unity/IUnityRenderingExtensions.h"
#include "Unity/IUnityLog.h"

//holding the unity log reference until plugin unloads
static IUnityLog* unityLogPtr = nullptr;
static uint8_t* dataBuffer = nullptr;

#define FORMAT(MESSAGE) std::string("[" + std::string(__FILE__) + ":" + std::to_string(__LINE__) + "] " + MESSAGE).c_str()
#define DEBUG_LOG(PTR, MESSAGE) if (nullptr != PTR ) UNITY_LOG(PTR, FORMAT(MESSAGE))
#define DEBUG_WARNING(PTR, MESSAGE) if (nullptr != PTR ) UNITY_LOG_WARNING(PTR, FORMAT(MESSAGE))
#define DEBUG_ERROR(PTR, MESSAGE) if (nullptr != PTR ) UNITY_LOG_ERROR(PTR, FORMAT(MESSAGE))
extern "C"
{
    UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfacesPtr)
    {
        //Get the unity log pointer once the Unity plugin gets loaded
        unityLogPtr = unityInterfacesPtr->Get<IUnityLog>();
        size_t size = 4;
        if (nullptr == dataBuffer)
        {
            dataBuffer = (uint8_t*)malloc(size);
            DEBUG_LOG(unityLogPtr, "dataBuffer malloc done");
        }
    }

    UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API UnityPluginUnload()
    {
        UNITY_LOG(unityLogPtr, "Plasma.UnityPluginUnload");
        //Clearing the log ptr on unloading the plugin
        unityLogPtr = nullptr;
    }

    UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API ManualUnityPluginLoad(uint64_t unityInterfacesPtr)
    {
        IUnityInterfaces* unityInterfaces = reinterpret_cast<IUnityInterfaces*>(unityInterfacesPtr);
        if (unityInterfaces != nullptr)
        {
            UnityPluginLoad(unityInterfaces);
        }
    }

    // UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API TestLog()
    // {
    //     //Generate different type of logs on unity console
    //     UNITY_LOG(unityLogPtr, "log from dll");
    //     UNITY_LOG_WARNING(unityLogPtr, "log warning from dll");
    //     UNITY_LOG_ERROR(unityLogPtr, "log error from dll");

    //     //Wrapper on above loggers to provide more info in Logs regarding file and line number
    //     DEBUG_LOG(unityLogPtr, "more informative log from dll");
    //     DEBUG_WARNING(unityLogPtr, "more informative warning from dll");
    //     DEBUG_ERROR(unityLogPtr, "more informative error warning from dll");
    // }

    // Old school plasma effect
    uint32_t Plasma(int x, int y, int width, int height, unsigned int frame)
    {
        float px = (float)x / width;
        float py = (float)y / height;
        float time = frame / 60.0f;

        float l = sinf(px * sinf(time * 1.3f) + sinf(py * 4 + time) * sinf(time));

        uint32_t r = sinf(l *  6) * 127 + 127;
        return r + 0xff000000u;
        // uint32_t g = sinf(l *  7) * 127 + 127;
        // uint32_t b = sinf(l * 10) * 127 + 127;

        // return r + (g << 8) + (b << 16) + 0xff000000u;
    }

    // Callback for texture update events
    void TextureUpdateCallback(int eventID, void *data)
    {
        if (eventID == kUnityRenderingExtEventUpdateTextureBeginV2)
        {
            // UpdateTextureBegin: Generate and return texture image data.
            UnityRenderingExtTextureUpdateParamsV2 *params = (UnityRenderingExtTextureUpdateParamsV2*)data;
            size_t size = params->width * params->height;
            
            DEBUG_LOG(unityLogPtr, "update textBuffer");
            unsigned int frame = params->userData;
            if (nullptr == dataBuffer)
            {
                DEBUG_ERROR(unityLogPtr, "dataBuffer is null");
            }
            else
            {
                for (unsigned int y = 0; y < params->height; y++)
                    for (unsigned int x = 0; x < params->width; x++)
                    {
                        // uint8_t v = (frame + (y * params->width + x) * 10) % 100 > 50 ? 0x66u : 0xffu;
                        uint8_t v = dataBuffer[y * params->width + x];
                        v+=y * params->width + x;
                        // DEBUG_LOG(unityLogPtr, "set texBuffer[" + std::to_string(y * params->width + x) + "] to " + std::to_string(v));
                        dataBuffer[y * params->width + x] = v;
                            // Plasma(x, y, params->width, params->height, frame);
                    }
            }

            uint8_t* tmpImgBuffer = (uint8_t*)malloc(size);
            std::memcpy(tmpImgBuffer, dataBuffer, size);
            DEBUG_LOG(unityLogPtr, "set params->texData");
            params->texData = tmpImgBuffer;
        }
        else if (eventID == kUnityRenderingExtEventUpdateTextureEndV2)
        {
            // UpdateTextureEnd: Free up the temporary memory.
            UnityRenderingExtTextureUpdateParamsV2 *params = (UnityRenderingExtTextureUpdateParamsV2*)data;
            free(params->texData);
        }
    }

    UNITY_INTERFACE_EXPORT UnityRenderingEventAndData UNITY_INTERFACE_API GetTextureUpdateCallback()
    {
        return TextureUpdateCallback;
    }
}