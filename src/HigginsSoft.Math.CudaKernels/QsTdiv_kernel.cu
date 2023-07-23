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
QsTDiv(int* n, int* bsmooth, int* gf2, int* div, const int* primes, int N, int P)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	if (i < N)
	{
		int mask = 0;
		for (int j = 0; n[i] > 1 && j < P; j++)
		{
			mask = 1 << j;
			while (n[i] % primes[j] == 0)
			{
				n[i] /= primes[j];
				gf2[i] ^= mask;
				div[i] |= mask;
			}
		}
		bsmooth[i] = n[i] == 1 ? 1 : 0;
	}
}
