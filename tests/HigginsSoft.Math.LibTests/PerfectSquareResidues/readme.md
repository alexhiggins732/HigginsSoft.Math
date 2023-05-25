A square residue is calculated as `i*i%n`

Let n be a given prime, then for all `i > sqrt(n)` primes have perfect square residues of count `floor(sqrt(prime))`

|N	|SquareResidues	|Sqrt				|ISqrt	|Density of Square Residues	|
|--	|--				|-------------------|1		|0.707106781186548			|
|2 	|0 				|1.4142135623731	|1		|0.707106781186548			|
|3 	|1 				|1.73205080756888	|1		|0.577350269189626			|
|5 	|2 				|2.23606797749979	|2		|0.447213595499958			|
|7 	|2 				|2.64575131106459	|2		|0.377964473009227			|
|11	|3 				|3.3166247903554	|3		|0.301511344577764			|
|13	|3 				|3.60555127546399	|3		|0.277350098112615			|
|17	|4 				|4.12310562561766	|4		|0.242535625036333			|
|19	|4 				|4.35889894354067	|4		|0.229415733870562			|
|23	|4 				|4.79583152331272	|4		|0.208514414057075			|
|29	|5 				|5.3851648071345	|5		|0.185695338177052			|
|31	|5 				|5.56776436283002	|5		|0.179605302026775			|
|37	|6 				|6.08276253029822	|6		|0.164398987305357			|


Specifically the count of square residues is the number of squares less than the prime.

|N	|Count	| Sq Residues
|2	|0		|
|3	|0		|
|5	|1		|4
|7	|1		|4
|11	|2		|9		4
|13	|2		|9		4
|17	|3		|16		9	4
|19	|3		|16		9	4
|23	|3		|16		9	4
|29	|4		|25		16	9	4

Including all i < n for a prime p, the total of square residues is 2 times number of squares less than p,
as naturally there are k square values less than i, and j values that are perfect squares mod p.

This pattern does not follow for composites. While the set of residues includes all squares less than some composite n,
some composites have multiple perfect square residues for a given square and some residues can be 0 mod n.


|N	|Count	| Sq Residues
|2	|0		|
|3	|0		|
|4	|1		|0
|5	|1		|4
|6	|1		|4
|7	|1		|4
|8	|2		|0	4
|9	|3		|0	0	4
|10	|2		|9	4
|11	|2		|9	4
|12	|5		|4	0	4	9	4
|13	|2		|9	4
|14	|2		|9	4
|15	|4		|4	4	9	4
|16	|9		|0	9	4	0	4	9	0	9	4
|17	|3		|16	9	4
|18	|6		|0	9	0	16	9	4
|19	|3		|16	9	4
|20	|10		|16	9	4	0	4	9	16	16	9	4
|21	|7		|4	16	16	4	16	9	4
|22	|3		|16	9	4
|23	|3		|16	9	4
|24	|10		|16	9	4	0	4	9	16	16	9	4
|25	|7		|0	0	0	0	16	9	4
|26	|4		|25	16	9	4
|27	|10		|9	0	9	9	0	9	25	16	9	4
|28	|13		|25	16	9	4	0	4	9	16	25	25	16	9	4

In particular:
- 4 is 0 mod 3^2
- 6 is 4 mod 4^2
- 8 is 0 mod 4 ^ 2 and 4 mod 6 ^ 2
- 9 is 0 mod 3 ^ 2 and 6 ^ 2, and 4 mod 7^2
- 10 is 9 mod 7 ^ 2 and 4 mod 8 ^ 2
- 12 is 4 mod 4 ^ 2, 0 mod 6 ^2, 4 mod 8 ^ 2, 9 mod 9 ^ 2, 4 mod 10 ^ 2
- 14 is 9 mod 11 ^ 2 and 4 mod 12 ^ 2
- 15 is 4 mod 7 ^ 2, 4 mod 8 ^ 2, 9 mod 12 ^ 2, 4 mod 13 ^ 2

See also:
- [Residues of all inteegers up to 255](QuadraticResidues-255.md)
- [Residues of primes up to 255](QuadraticResiduesPrime-255.md)