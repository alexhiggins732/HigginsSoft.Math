A mersenne number is a number in the form of 2^N-1, typically denoted as MPN.

There are various interesting properties of mersenne numbers from a number theory and computation perspective.

Computationally, mpN can be caculated as (1<<n) - 1. In binary, the representation is n 1 bits.

	For example, MP5 = 31:
		1<<5 = 100000 - 1  = 11111
	So we have MP5 = 1<< 5 - 1 = 11111

Modular arithmetic of 2^N can be caculuated using the binary AND operation of dividend and 2^N-1. That is c mod 2^n = the lowest N bits of c.

	For example, MP5 = 31:
		mp = 1<<5
		n = 27
			= 0001 1011
			= 0001 0000
		n^2 = 729
			= 0010 1101 1001

		n^2 = 729 & 31 = 1 1001 = 25 = 
			= 0010 1101 1001 // n^2 
			= 0000 0001 1111 // 2^5-1
			= 0000 0001 1001 // n^2 & mp5
	

Division 2 ^n accomplish using the right shift operator.

	For example, MP5 = 31:
		mp = 1<<5-1
		n = 27
			= 0001 1011
			= 11111
		n^2 = 729
			= 0010 1101 1001
		n^2 / (2^n) = 729 >> 5 = 22  = 0001 0110
			= 0010 1101 1001
			= 0000 0001 011001 1001 (right shift 5)
			= 0000 0001 0110 (we lost all bits to the right of the decimal place)

Combining the two operations leads to an efficient algorithm for performing modular arithmetic mod 2^N-1 as follows:

	while (c > 2^N-1)
		c = (c & 2^N-1) + (c>>n)


	For example, MP5 = 31:
		mp = 1<<5-1
		n = 27
			= 0001 1011
			= 11111
		n^2 = 729
			= 0010 1101 1001

	let c = n^2, to perform c mod mp5 (2^5-1) we have:

		Iteration 1:
			while (c>MP5)		// true = 729 > 31
				low = c & MP5	// 25 = 729 & 31
				high = c >> 5	// 22 = 729 >> 5
				c = low + high	// 47 = 25 + 22

		Iteration 2:
			while (c>MP5)		// true = 47 > 31
				low = c & MP5	// 15 = 47 & 31
				high = c >> 5	// 1 = 47 >> 5
				c = low + high	// 16 = 15 + 1

		Iteration 3:
			while (c>MP5)		// false = 16 > 31

		Result = or 729 mod 31 = 16

	So we have 27^2 mod 2^5-1 = 16, or 729 mod 31 = 16


Consider using base 10, instead of 2, to see how this works to gain a better understanding of the alogirth. 

Setting N=2, as an arbitrarty example, we have MP2 = 10^2 = 100 and MP2-1=99.


	Reusing the previous dividences of 27 and 27^2 = 729:

		To perform division, we simply shift c right by N bits. Since N=2 we have

			729 / 10^2 = 729 >> 2 = 7.29 = 7 (Since we are working Z, we simply trunace the decimals)
	
		To perform the modulus operation we take cand 10^2-1 (99) which we can simply state as taking the lowest N bits.

			729 % 10^2 = 29.


		Now, using the same algorithm to perform the modulus operation:

		let c = n^2, to perform c mod 10^2-1 (99) we have:

			Iteration 1:
				while (c>MP-1)		// true = 729 > 99
					low = c & MP-1	// 29 = 729 & 99
					high = c >> 2	// 7 = 729 >> 2
					c = low + high	// 36 = 29 + 7

			Iteration 2:
				while (c>MP-1)		// false = 36 > 99

			result: 729 % 99 = 36



1001
1001

1001
1100
-----
10101
%1111
 0110
 
0000 1001
0000 1001
---------
0000 1001 9
0100 1000 72
---------
0101 0001  81
%    mod 15 = n&15 +n>>4 (while n>np) n= n&mpMask + (n>>mpBits)
  0000 0001
+ 0000 0101
----------
= 0000 0110

long form

  0000 1001
  0000 1001
-----------
  0000 1001  9 * 1 // += n<<0 * n&(1<<0)
  0001 0010 18 * 0 // += n<<1 * n&(1<<1)
  0010 0100 36 * 0 // += n<<2 * n&(1<<2)
  0100 1000 72 * 1 // += n<<3 * n&(1<<3)
-----------

	Note: as full arrays, a matter of inserting 0
	as index, can just calculate

rls = rotate left shift
direct routing
0000 1001
rls(0000 1001) = 0000 0011
rls(0000 1001) = 0000 0110
rls(0000 1001) = 0000 1100
0000 1001
0000 1100 add
----------
0001 0101 carry

0000 0101 
0000 0001 add
---------
0000 0110 6	

direct routing long form

  0000 1001
  0000 1001
-----------
  0000 1001  9 * 1 // += (n<<0&mpMask)| (n>>mpbits-0) * n&(1<<0)
  0000 0011  6 * 0 // += (n<<1&mpMask)| (n>>mpbits-1) * n&(1<<1)
  0000 0110 12 * 0 // += (n<<2&mpMask)| (n>>mpbits-2) * n&(1<<2)
  0000 1100 24 * 1 // += (n<<3&mpMask)| (n>>mpbits-3) * n&(1<<3)
-----------

	Note: as full arrays, shifts/rotates for each word can be precomputed
	as index, operations for inputs/outputs can be cacluated.


avx256: 4 i64, or 8 i32, 16 31, 32 bytes
	number of adds per bit=n
	n32= done on machine with single operationsn
	n=64%10^6 (2^64,000,000)
	 64M adds: Options:
		1) represent each word use smaller word bounds to allow for overflow
			eg 32 bits of 64 to allow for overflow, or 16 bits of 32
		2) perform 2 operations, one for low, one for high.
	
	 64M=2M i32s,4M to allow for carry, either using 64/32 or 32/16
	 avx256/512._mm256_add_epi8/16/32/64
		with _mm256_add_epi64 (u64,u64)= 64M/4 = 16M add+carryops per iter
		latency 1, thoughput=.33, so we can do 3 _mm256_add_epi64 operations in a single cycle
			=> Computing 16M add ops at 3 ops per cycle = 5.28M cycles
			=> CPU clock speed -> 3.7MHZ 
				=> 1.4270 seconds per op, per core. 5.28M cycles / 3.7mhz
					=> 16 cores = 0.0891875 seconds per cycle
					=> 32 cores = 0.04459375 seconds per cycle
					=> 995.81 cores = 0~.001433 seconds
				-> openGPUOWL => 1433 us/it or 0.001433 seconds
		with mm512_add_epi64 (u64,u64)= 64M/8 = 8M add+carryops per iter
			latency 1, thoughput=.5, so 2 ops per second.
			=> Computing 8M add ops at 2 ops per cycle = 4M cycles
			=> CPU clock speed -> 3.7MHZ
				=> 1.081... seconds per op, per core. 4M cycles / 3.7mhz
					=> 16 cores = 0.0675625 seconds per cycle
					=> 32 cores = 0.00253125 seconds per cycle
					=> 754.36 cores = 0~.001433 seconds
				-> openGPUOWL => 1433 us/it or 0.001433 seconds


Routing:

	B = b[n-1], b[...], b[0]
	A = a[n-1], a[...], a[0]

	c[0]	= (a[0] * b[0])
	c[1]	= (a[1] * b[0])
	c[...]	= (a[...] * b[0])
	c[n-1]	= (a[n-1] * b[0])

	
	2d array:
	  options: (threads can be scheduled have blocks work in c[i] and block threads partition c[i][k])
		c[ai][bi]	= a[ai] * b[bi]
		c[bi][ai]	= a[ai] * b[bi]

	1d array: (Will there be memory contention in 1d?)
		c[bi*n+ai]	= a[ai] * b[bi]
		c[ai*n+bi]	= a[ai] * b[bi] 

	C can be reused, but needs to be cleared on each iteration.

Options:
	[low,high]
	c[][] => parallelism
	carry loop
