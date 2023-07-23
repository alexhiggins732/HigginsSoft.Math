/*
 * Copyright 1993-2015 NVIDIA Corporation.  All rights reserved.
 *
 * Please refer to the NVIDIA end user license agreement (EULA) associated
 * with this source code for terms and conditions that govern your use of
 * this software. Any use, reproduction, disclosure, or distribution of
 * this software and related documentation outside the terms of the EULA
 * is strictly prohibited.
 *
 */

 /**
  * CUDA Kernel Device code
  *
  * Computes the vector addition of A and B into C. The 3 vectors have the same
  * number of elements numElements.
  */

/*
int dimX = 512;
int dimY = 512;
float[] array_host = new float[dimX * dimY];
CudaPitchedDeviceVariable<float> arrayPitched_d = new CudaPitchedDeviceVariable<float>(dimX, dimY);
for (int y = 0; y < dimY; y++)
{
    for (int x = 0; x < dimX; x++)
    {
        array_host[y * dimX + x] = x * y;
    }
}

arrayPitched_d.CopyToDevice(array_host);
kernel.Run(arrayPitched_d.DevicePointer, arrayPitched_d.Pitch, dimX, dimY);
*/
//Correspondend kernel:
extern "C"
__global__ void KernelPitched(float* data, size_t pitch, int dimX, int dimY)
{
    int x = blockIdx.x * blockDim.x + threadIdx.x;
    int y = blockIdx.y * blockDim.y + threadIdx.y;
    if (x >= dimX || y >= dimY)
        return;

    //pointer arithmetic: add y*pitch to char* pointer as pitch is given in bytes,
    //which gives the start of line y. Convert to float* and add x, to get the
    //value at entry x of line y:
    float value = *(((float*)((char*)data + y * pitch)) + x);

    *(((float*)((char*)data + y * pitch)) + x) = value + 1;

    //Or simpler if you don't like pointers:
    float* line = (float*)((char*)data + y * pitch);
    float value2 = line[x];
}