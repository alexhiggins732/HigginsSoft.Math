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

extern "C" __global__ void
vectorAdd(const unsigned long long*A, const unsigned long long*B, unsigned long long*C, int numElements)
{
    int i = blockDim.x * blockIdx.x + threadIdx.x;
    for (int j = 0; j < 250000; j++)
    {
        if (i < numElements)
        {
            C[i] = A[i] + B[i];
        }
    }
}

