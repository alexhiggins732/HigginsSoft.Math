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

/*
float[][] data_h;
CudaDeviceVariable<CUdeviceptr> data_d;
CUdeviceptr[] ptrsToData_h; //represents data_d on host side
CudaDeviceVariable<float>[] arrayOfarray_d; //Array of CudaDeviceVariables to manage memory, source for pointers in ptrsToData_h.

int sizeX = 512;
int sizeY = 256;

data_h = new float[sizeX][];
arrayOfarray_d = new CudaDeviceVariable<float>[sizeX];
data_d = new CudaDeviceVariable<CUdeviceptr>(sizeX);
ptrsToData_h = new CUdeviceptr[sizeX];
for (int x = 0; x < sizeX; x++)
{
    data_h[x] = new float[sizeY];
    arrayOfarray_d[x] = new CudaDeviceVariable<float>(sizeY);
    ptrsToData_h[x] = arrayOfarray_d[x].DevicePointer;
    //ToDo: init data on host...
}
//Copy the pointers once:
data_d.CopyToDevice(ptrsToData_h);

//Copy data:
for (int x = 0; x < sizeX; x++)
{
    arrayOfarray_d[x].CopyToDevice(data_h[x]);
}

//Call a kernel:
kernel.Run(data_d.DevicePointer , other parameters);

//kernel in *cu file:
//__global__ void kernel(float** data_d, ...)
*/
//Correspondend kernel:
extern "C"
__global__ void KernelJagged(float* data, size_t pitch, int dimX, int dimY)
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