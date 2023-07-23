## Primes Mod 2

### Lemma 1:

If N = 0 mod 2, n is only prime if n = 2.

#### Proof:

Assume N is a number such that N = 0 mod 2. We will prove that n is only prime if n = 2.

First, let's consider the definition of a prime number. A prime number is a natural number greater than 1 that has no positive divisors other than 1 and itself.

If N = 0 mod 2, it means N is an even number. By definition, an even number is divisible by 2 without leaving a remainder.

Now, let's consider the possible divisors of N. Since N is even, it can be expressed as N = 2 * k, where k is an integer.

If n is a prime number, it should only have two divisors, 1 and itself. However, since N is even, it has at least three divisors: 1, 2, and N/2.

For example, let's consider N = 4. In this case, the divisors of 4 are 1, 2, and 4. Since it has more than two divisors, it cannot be considered a prime number.

This applies to any even number greater than 2. Therefore, if N = 0 mod 2, n can only be prime if n = 2.

Hence, Lemma 1 is proven.


### Lemma 2:

If N = 0 mod 2, then N is not a semiprime unless N = 4.

#### Proof:

To prove this lemma, we will consider the definition of a semiprime and analyze the properties of even numbers.

A semiprime is a natural number that is the product of two prime numbers. It can be expressed as N = p * q, where p and q are prime numbers.

If N = 0 mod 2, it means N is an even number. By definition, an even number is divisible by 2 without leaving a remainder.

Now, let's consider the possible factors of N. Since N is even, it can be expressed as N = 2 * k, where k is an integer.

If N is a semiprime, it should have two prime factors, p and q. However, since N is divisible by 2, one of its prime factors must be 2.

If p = 2, then N = 2 * q. In this case, N would be a multiple of 2 and cannot be a semiprime.

If q = 2, then N = p * 2. Similarly, N would be a multiple of 2 and cannot be a semiprime.

Therefore, if N = 0 mod 2, N cannot be a semiprime unless N = 4, where N is the product of two prime factors 2 and 2.

Hence, Lemma 2 is proven.

### Lemma 3:

If N is a semiprime, with primes divisors p and q, then N = 1 mod 2 unless p or q = 2.

#### Proof

From Lemma 1, we know that if N is even (N = 0 mod 2), then N is not a semiprime unless N = 4. 

Therefore, it is trivial to show by induction that any semiprime congruent to 0 mod 2 must have prime factor of 2.

Hence, Lemma 3 is proven.

### Lemma 4:

If N is a semiprime, with primes divisors p and q > 2, then p = 1 mod 2 and q = 1 mod 2.

### Proof:

We will prove Lemma 3 by induction, building upon the proof of Lemma 1 and Lemma 2.

Let's assume N is a semiprime, which means N = p * q, where p and q are prime factors of N.

From Lemma 1, we know that if N is even (N = 0 mod 2), then N is not a semiprime unless N = 4.

From Lemma 2 we know that a semiprime N must be = 1 mod 2.

By the fundamental theorem of arithmetic, the product of an even number and an even number is even and the product of an even number and an odd number is even. The product of an odd number and an odd number is also odd.

Consider the cases for the divisors of a semiprime:

**Case 1:**
p is even and q is even
If we suppose p is even and q is even then we know from Lemma 1 that p must equal 2 and q must equal 2.
**Case 2:**
p is even and q is odd
By the fundamental theorom of arithmetic N will be even as a product of an even and odd number.
Building upon Lemma 2 it is trivial to show by induction if N is a semiprime with an even divisor p, then p must equal 2 and q must be an odd prime.
**Case 3:**
p is even and q is odd
As a juxtoposition of case 3, by the fundamental theorom of arithmetic N will be an even number, as a product of an odd and even number is even.
As with cas 2, it is then trivial to sho that N is a semiprime with an even divisor q, and odd divisor q, then q must equal 2 and p must be an odd prime.



If we suppose p is even and q is even then we know from Lemma 1 that p must equal 2 and q must equal 2.

Therefore, for the base case of the induction, we will consider N to be a odd semiprime.


Based on the above cases, we can conclude that if N is a semiprime, then the prime divisors p and q of N are both congruent to 1 modulo 2.

By induction, we have shown that Lemma 2 holds true.

Hence, Lemma 3 is proven.

### Lemma 4:

If N is a semiprime without a prime divisor of 2, then both p and q are both odd primes.

#### Proof Lemma 4:

By induction, Lemma 4 is proven in Lemma 3. As proven in Lemma 3, if N is a semiprime, with primes divisors p and q > 2, then p = 1 mod 2 and q = 1 mod 2.

By definition, a semiprime consists of only two divisors. As we have proven in Lemma 3 those divisors must be odd unless p or q = 2, lemma 4 is proven by corrally of Lemma 3.




## Semiprimes mod 3:

### Lemma 1:

If N = 0 mod 3, then N is not a semiprime unless N = 9.

#### Proof

We show that if N = 0 mod 3, then N is not a semiprime unless N = 9.

Assume N is a semiprime where N = 0 mod 3 and N ≠ 9. Let N = p * q, where p and q are prime factors of N.

Since N = 0 mod 3, we have three cases for the values of p and q:

Case 1: p = 0 mod 3 and q = 0 mod 3.
In this case, both p and q are divisible by 3. Since 3 is a prime number, this implies that N is divisible by 3^2 = 9. 
However, we assumed N ≠ 9, which contradicts our assumption. Therefore, this case is not possible.
We also note that N in this case would be a perfect power of 3 and given the assumption N ≠ 9 that N is not a perfect square of a prime power.
We will extend our proofs based on N being a perfect power of a prime with the exception of N being a square later.

Case 2: p = 0 mod 3 and q ≠ 0 mod 3.
In this case, p is divisible by 3, but q is not divisible by 3. 
Since q is a prime number, it must be congruent to either 1 or 2 mod 3.
However, this contradicts the fact that N = p * q is congruent to 0 mod 3. Therefore, this case is not possible.

Case 3: p ≠ 0 mod 3 and q = 0 mod 3.
This case is a juxtoposition of Case 2 but with the roles of p and q reversed. Similarly, we can conclude that this case is not possible.

Since all three cases lead to contradictions, we can conclude that if N = 0 mod 3 and N ≠ 9, then N is not a semiprime.

Therefore, Lemma 1 is proven.

### Lemma 2:

N can only be a semiprime if N = 1 mod 3 or N = 2 mod 3.



#### Proof:

We want to show that N can only be a semiprime if N = 1 mod 3 or N = 2 mod 3.

Assume N is a semiprime. We know that N = p * q, where p and q are prime factors of N.

Since N is a semiprime, both p and q must be greater than 1. Now, consider the possible s of p and q modulo 3:

**Case 1:**
If p = 0 mod 3 or q = 0 mod 3, then from Lemma 1, we have already shown that N cannot be a semiprime unless N = 9. Therefore, this case is not possible.

**Case 2:**
If p = 1 mod 3 and q = 1 mod 3, then N = p * q is congruent to 1 * 1 = 1 mod 3.

**Case 3:**
If p = 2 mod 3 and q = 2 mod 3, then N = p * q is congruent to 2 * 2 = 4 mod 3, which is equivalent to 1 mod 3.

**Case 4:**
If p = 1 mod 3 and q = 2 mod 3, then N = p * q is congruent to 1 * 2 = 2 mod 3.

**Case 5:**
If p = 2 mod 3 and q = 1 mod 3, then N = p * q is congruent to 2 * 1 = 2 mod 3.

**Case 6:**
If p = 0 mod 3 and q ≠ 0 mod 3:
In this case, p is divisible by 3, but q is not divisible by 3. Since q is a prime number, it must be congruent to either 1 or 2 mod 3. 
The product N = p * q will be congruent to 0 * 1 = 0 mod 3 or 0 * 2 = 0 mod 3, which contradicts the fact that N is congruent to 1 or 2 mod 3. Therefore, this case is not possible.

**Case 7:**
If p ≠ 0 mod 3 and q = 0 mod 3:
This case is similar to Case 1 but with the roles of p and q reversed. Again, we can conclude that this case is not possible.

**Case 8:**
If both p and q are congruent to 0 mod 3:
In this case, both p and q are divisible by 3. Since 3 is a prime number, this implies that N is divisible by 3^2 = 9. 
However, we assumed N is not equal to 9, which contradicts our assumption. Therefore, this case is not possible.

**Case 9:**
If p ≠ 0 mod 3 and q ≠ 0 mod 3:
In this case, both p and q are not divisible by 3. Since p and q are prime factors of N, they cannot be congruent to 0 mod 3. Therefore, we can conclude that both p and q must be congruent to either 1 mod 3 or 2 mod 3.

By considering all possible combinations of congruences (1 mod 3 or 2 mod 3) for p and q, we have covered all cases noting 4 non-trivial cases:

**Case 2:** p = 1 mod 3 and q = 1 mod 3
**Case 3:** p = 2 mod 3 and q = 2 mod 3
**Case 4:** p = 1 mod 3 and q = 2 mod 3
**Case 5:** p = 2 mod 3 and q = 1 mod 3


**Case 1:** If p = 0 mod 3 or q = 0 mod 3, then N cannot be a semiprime unless N = 9.
**Cases 2, 3, 4, 5**: These cases cover the situations where p and q are congruent to either 1 mod 3 or 2 mod 3.
**Cases 6, 7, 8:** These cases are not possible, as discussed.

Therefore, the cases are now complete, and we have shown that if N is a semiprime, it can only fall into one of the four non-trivial cases mentioned above.

Thus, we have shown that if N is a semiprime, it can only be congruent to 1 mod 3 or 2 mod 3.

Therefore, Lemma 2 is proven.


## Representation of semiprimes mod 4

Lemma y:

Zero Product Property of Modular Arithmetic:

For any integers P and Q, if P ≡ 0 (mod 3) or Q ≡ 0 (mod 3), then P * Q ≡ 0 (mod 3).

Proof (Lemma y):

We prove Lemma y by examining the possible cases based on the  classes of P and Q modulo 3:

Case 1: P ≡ 0 (mod 3)

In this case, we have P = 3k for some integer k. Then, P * Q = (3k) * Q = 3(k * Q), which is divisible by 3. Therefore, P * Q ≡ 0 (mod 3).

Case 2: Q ≡ 0 (mod 3)

Similarly, if Q ≡ 0 (mod 3), we have Q = 3m for some integer m. Then, P * Q = P * (3m) = 3(P * m), which is divisible by 3. Hence, P * Q ≡ 0 (mod 3).

Case 3: P ≡ 0 (mod 3) and Q ≡ 0 (mod 3)

In this case, both P and Q are divisible by 3, say P = 3k and Q = 3m for some integers k and m. Then, P * Q = (3k) * (3m) = 9(k * m) = 3(3(k * m)), which is divisible by 3. Thus, P * Q ≡ 0 (mod 3).

Based on the zero product property of modular arithmetic, we have shown that if P ≡ 0 (mod 3) or Q ≡ 0 (mod 3), then P * Q ≡ 0 (mod 3). This property holds for any integers P and Q.

Hence, Lemma y is proven.


Lemma 3:

If N is a semiprime with divisors p and q ≠ 0 mod 2 and ≠ 0 mod 3

We examine the cases:

There are 9 unique cases, give that 3 *3 = 9
 - P = 0 (mod 3) * Q = 0 (mod 3) = 0 (mod 3) = 0
 - P = 0 (mod 3) * Q = 1 (mod 3) = 0 (mod 3) = 0
 - P = 0 (mod 3) * Q = 2 (mod 3) = 0 (mod 3) = 0
 - P = 1 (mod 3) * Q = 0 (mod 3) = 0 (mod 3) = 0
 - P = 1 (mod 3) * Q = 1 (mod 3) = 0 (mod 3) = 1
 - P = 1 (mod 3) * Q = 2 (mod 3) = 0 (mod 3) = 2
 - P = 2 (mod 3) * Q = 0 (mod 3) = 0 (mod 3) = 0
 - P = 2 (mod 3) * Q = 1 (mod 3) = 0 (mod 3) = 2
 - P = 2 (mod 3) * Q = 2 (mod 3) = 4 (mod 3) = 1

As we expand our proofs, we note the Zero Product Property of Modular Arithmetic:  
 - P = 0 (mod 3) * Q = 0 (mod 3) = 0 (mod 3) = 0
 - P = 0 (mod 3) * Q = 0 (mod 3) = 0 (mod 3) = 0
 - P = 0 (mod 3) * Q = 1 (mod 3) = 0 (mod 3) = 0
 - P = 0 (mod 3) * Q = 2 (mod 3) = 0 (mod 3) = 0
 - P = 1 (mod 3) * Q = 0 (mod 3) = 0 (mod 3) = 0
 - P = 2 (mod 3) * Q = 0 (mod 3) = 0 (mod 3) = 0

We provide formal proof Lemma Y and note the propery holds for all numbers which includes the semiprime subset of numbers being examined.

Now we examine the non-trivial subset of s not reduced by Zero Product Property:

Case N = 1 mod 3:

There are two cases:
-
	 P = 1 mod 3 * Q = 1 mod 3 = 1 mod 3 = 1
	 P = 2 mod 3 * Q = 2 mod 3 = 4 mod 3 = 1

Case N = 2 mod 3:

There are two cases:
-
	P = 1 mod 3 * Q = 2 mod 3 = 2 mod 3 = 2
	P = 2 mod 3 * Q = 1 mod 1 = 2 mod 3 = 1

Therefore, Lemma 3 has been proven while providing inductive basis for reducing additional  classes based on the Zero Product Property.


Lemma 4:

If N is a semiprime with divisors p and q such that p ≢ 0 (mod 2) and q ≢ 0 (mod 2), and p ≢ 0 (mod 3) and q ≢ 0 (mod 3), then N ≢ 0 (mod 3).

Proof (Lemma 4):

We begin by examining the nine unique cases based on the  classes of p and q modulo 3, as stated in Lemma 3.

For each case, we apply the Zero Product Property of Modular Arithmetic, which states that if P ≡ 0 (mod 3) or Q ≡ 0 (mod 3), then P * Q ≡ 0 (mod 3).

By noting the Zero Product Property, we can conclude the following:
-
	P ≡ 0 (mod 3) * Q ≡ 0 (mod 3) ≡ 0 (mod 3) ≡ 0 (mod 3)
	P ≡ 0 (mod 3) * Q ≡ 0 (mod 3) ≡ 0 (mod 3) ≡ 0 (mod 3)
	P ≡ 0 (mod 3) * Q ≡ 1 (mod 3) ≡ 0 (mod 3) ≡ 0 (mod 3)
	P ≡ 0 (mod 3) * Q ≡ 2 (mod 3) ≡ 0 (mod 3) ≡ 0 (mod 3)
	P ≡ 1 (mod 3) * Q ≡ 0 (mod 3) ≡ 0 (mod 3) ≡ 0 (mod 3)
	P ≡ 2 (mod 3) * Q ≡ 0 (mod 3) ≡ 0 (mod 3) ≡ 0 (mod 3)

Therefore, we have established that if either P or Q is congruent to 0 (mod 3), then P * Q is congruent to 0 (mod 3) based on the Zero Product Property.

Next, we examine the non-trivial subset of s that are not reduced by the Zero Product Property. These are the cases where both P and Q are non-zero s modulo 3.

For the case N ≡ 1 (mod 3), we have the following:
- 
	 P ≡ 1 (mod 3) * Q ≡ 1 (mod 3) ≡ 1 (mod 3)
	 P ≡ 3 (mod 3) * Q ≡ 3 (mod 3) ≡ 4 (mod 3) ≡ 1 (mod 3)

And for the case N ≡ 2 (mod 3), we have:
-
	 P ≡ 1 (mod 3) * Q ≡ 2 (mod 3) ≡ 2 (mod 3)
	 P ≡ 2 (mod 3) * Q ≡ 1 (mod 3) ≡ 1 (mod 3)

In each of these cases, we can see that the product P * Q is not congruent to 0 (mod 3).

Therefore, we have proven Lemma 3 by examining all possible cases and demonstrating that if N is a semiprime with divisors p and q such that p ≢ 0 (mod 2) and q ≢ 0 (mod 2), and p ≢ 0 (mod 3) and q ≢ 0 (mod 3), then N ≢ 0 (mod 3). Additionally, we have shown that the Zero Product Property holds for the given  classes, further supporting the proof of Lemma 3.



## Semiprimes mod 4

There exists 4 classes mod 4: 0,1,2,3

### Lemma 1
  If n = 0 mod 4 then n is not a semi prime unless n=4


#### Proof:

Assume n is a number such that n = 0 mod 4, where n is not equal to 4. We will show that n cannot be a semiprime.

Since n = 0 mod 4, it implies that n is divisible by 4. This means n can be expressed as n = 4k, where k is an integer.

Now, let's consider the factors of n. Any factor of n must divide n evenly. Since n = 4k, the factors of n can be expressed as 1, 2, k, and 4k.

Note that 2 is a factor of n, and since 4 is also a factor of n, n can be expressed as the product of two distinct prime factors (2 and 2), making it a semiprime.

Therefore, the assumption that n is not a semiprime when n = 0 mod 4 is incorrect.

However, there is an exception when n = 4. In this case, n is a semiprime since it can be expressed as the product of two distinct prime factors (2 and 2).

Any other number that would exist in the form of n = 4k must have a distinct prime factorization of n = 2 * 2 * k, where k has one or more factors depending if it is prime.
Hence, the proof of Lemma 1 is complete.

### Lemma 2
  If n = 2 mod 4 then n is a semi prime if and only if n / 2 is prime

#### Proof

Assume n is a number such that n = 2 mod 4. We will prove that n is a semiprime if and only if n/2 is prime.

First, let's consider the case when n is a semiprime. This means n can be expressed as the product of two distinct prime factors, p and q, where p and q are both greater than 1. Thus, we have n = p * q.

Since n = 2 mod 4, we can write n as n = 4k + 2 for some integer k. Dividing both sides by 2, we get n/2 = 2k + 1.

If n/2 is not prime, it means there exists a factor f of n/2 such that 1 < f < n/2. Multiplying both sides of the inequality by 2, we have 2f < n, which implies that f is a factor of n. Since f is less than n/2, it cannot be equal to p or q (which are distinct prime factors of n). Therefore, n cannot be a semiprime, contradicting our initial assumption.

On the other hand, if n/2 is prime, it means there are no factors of n/2 other than 1 and n/2 itself. Multiplying both sides of the equation n/2 = 2k + 1 by 2, we have n = 4k + 2. This shows that n satisfies the condition n = 2 mod 4.

Therefore, n is a semiprime if and only if n/2 is prime.

Hence, the proof of Lemma 2 is complete.

### Lemma 3:
If n = 3 mod 4, then n is a semiprime if and only if n / 3 is prime.

#### Proof

Assume n is a number such that n = 3 mod 4. We will prove that n is a semiprime if and only if n / 3 is prime.

First, let's consider the case when n is a semiprime. By definition, a semiprime is a composite number that is the product of two prime factors, p and q, where p and q are both greater than 1. Thus, we have n = p * q.

Since n = 3 mod 4, we can write n as n = 4k + 3 for some integer k.

Now, let's examine the possible values of p and q. Since p and q are prime factors of n, they must satisfy p = 1 mod 4 and q = 1 mod 4, or p = 3 mod 4 and q = 3 mod 4.

If p = 1 mod 4 and q = 1 mod 4, their product p * q will also be 1 mod 4. However, this contradicts the fact that n = 3 mod 4.

If p = 3 mod 4 and q = 3 mod 4, their product p * q will be 9 mod 16, which is equivalent to 1 mod 4. This is consistent with the condition n = 3 mod 4.

Therefore, if n is a semiprime, the factors p and q must be of the form p = 3 mod 4 and q = 3 mod 4.

Now, let's consider n / 3. We have n / 3 = (p * q) / 3 = (3 mod 4) * (3 mod 4) / 3 = 1 mod 4.

This means that n / 3 is congruent to 1 mod 4.

If n / 3 is prime, it will satisfy the condition n / 3 = 1 mod 4. This implies that n is a semiprime.

Conversely, if n is a semiprime and n / 3 = 1 mod 4, then n / 3 must be prime, as it satisfies the condition for prime numbers in relation to modulo 4.

Therefore, we have shown that if n = 3 mod 4, n is a semiprime if and only if n / 3 is prime.

Hence, Lemma 3 is proven.

# Squares and Perfect Powers

### Lemma 1
If N is a perfect square, then the exponents of its prime factorization must be even.

#### Proof:

To prove Lemma 1, let's assume that N is a perfect square. We need to show that the exponents of its prime factorization are even.

Let's assume N is a perfect square N = m^2, then we have two cases, either m is a prime or m is composite. 

Case 1:

Suppose N = m^2 where m is a equal to prime p. The prime factorization of N can be expressed as N = p * p or N = p^2 and the exponent of its prime factorizations is even. 

Case 2:

If m is not prime, with k prime factors, then its prime factorization can be expressed as m = p[0]^e[0] * p[1]^e[1] * ... * p[k]^e[k], where e[0], e[1], ..., e[k] are non-negative integers representing the exponents of the prime factors in the factorization of m.

Since N = m * m, taking the square of we have N = p[0]^e[0] * p[1]^e[1] * ... * p[k]^e[k] * p[0]^e[0] * p[1]^e[1] * ... * p[k]^e[k]

This can be rewritte as m^2 = (p[0]^e[0])^2 * (p[1]^e[1])^2 * ... * (p[k]^e[k])^2.

Squaring any number given number z in the set of positive integers results in the exponent being doubled.

Therefore, each exponent in e[0], e[1], ..., e[k] in the prime factorization of m will be doubled.

To double the exponent we mulitply it by two.

By the fundemental theorom the result of mulitplying any number by an even number results in an even number.


Therefore, we have shown that if N is a perfect square, the exponents of its prime factorization must be even.

Hence, Lemma 1 is proven.


### Lemma 2

Let n be a perfect square. Then it n is only a semiprime only if the square root of n is a prime.
By corrally, no perfect squares exist that are semiprime except the case of N being the perfect square of a prime.

### Proof:

To prove Lemma 2, let's assume that n is a perfect square and it is also a semiprime. We need to show that the square root of n is a prime number.

By definition, a semiprime, can only be factorized into two prime factors, say p and q, such that n = p * q.

Suppose n = m^2, where m is a positive integer.

From this, we have m^2 = p * q. Since m^2 is a perfect square, it means that each prime factor on the right side, p and q, from Lemma 1, it shows we must have an even exponent in their prime factorizations.
This is because when we square m, each prime factor in the factorization of m will be doubled in its exponent.

Now, let's consider the square root of n. Taking the square root of both sides of m^2 = p * q, we have sqrt(m^2) = sqrt(p * q), which simplifies to m = sqrt(p * q).

Since m is an integer, it means that sqrt(p * q) is also an integer. This implies that p * q must be a perfect square. 
However, since both p and q have even exponents in their prime factorizations, their product p * q will also have even exponents for all prime factors.

But a perfect square can only have even exponents for its prime factors if and only if all of its prime factors are repeated with even exponents. 

In other words, p and q must be the same prime number.

Consider the cases

Case 1: 
If p and q are not the same prime number. Then N = p^2 * q^2 and would have 4 prime factors. This condicts the assumption that N is a semprime.

Case 2:
If and q are the same prime number, m, then we have N = m^2. This is the only valid case where N is a perfect square and a semiprime

Therefore, we can conclude that if n is a perfect square and a semiprime, its square root (m) must be a prime number.

Thus, Lemma 2 is proven.

### Lemma 3:
If N is perfect power, it can only be a semiprime if it is the perfect square of a prime. 
By corrally, no perfect power power exists that are semiprime except the case of N being the perfect square of a prime.

Proof:


To prove Lemma 2, let's assume that N is a perfect power and it is also a semiprime. 

We need to show that only semiprime that can exist that is a perfect power is one that is a perfect square of a prime.

To satisfy the conidition that is a semprime, we must have N = p * q.

To express a number i as perfect power to the power of x we write i^x.

Since a semiprime can only have two divisors, as a perfect power we must write N = i^x.

Otherwise, we would need N = p^xp[0] * q^xq[1].

Since i^0 = 1 it is not a prime, so we must have x for p and x > 0.

If we set N = p^1 * q^1, with p!=Q, then definition N is not perfect power.

To meet the semiprime condition for a perfect power, we are constrained to perfect powers of a prime and prove that we must write N = i^x.

Since i^0 is not a prime, we are constrained to i^x where x>0.

If we write, N = i^x where x is greater than 2, then we now have more than two divisors which contradicts the assumption N is a semiprime.

Hence we have on last case, the one where N = i^2 and i is a perfect prime p.

Therefore, there is only one condition which the assumption is valid that N is a perfect power and a semiprime is valid.

Thus, Lemma 3 is proven.


# Semiprime  Classes


We build a system of  classes denoted as C for each integer c greater than 1. 

That is for C, their exists k=c-1  classes, for which any integer i mod c belongs.

We examine the  class for each n mod c in the set of Semiprimes in N, and its relation to the  classes of prime divisors P and Q of n.

Without placing constraints on n, we observe.

We note there exists 24 unique semiprimes in N for c <100:
4 = 2 * 2
6 = 2 * 3
9 = 3 * 3
10 = 2 * 5
14 = 2 * 7
15 = 3 * 5
21 = 3 * 7
22 = 2 * 11
25 = 5 * 5
26 = 2 * 13
33 = 3 * 11
34 = 2 * 17
35 = 5 * 7
38 = 2 * 19
39 = 3 * 13
46 = 2 * 23
49 = 7 * 7
51 = 3 * 17
55 = 5 * 11
57 = 3 * 19
58 = 2 * 29
62 = 2 * 31
65 = 5 * 13
69 = 3 * 23
74 = 2 * 37
77 = 7 * 11
82 = 2 * 41
85 = 5 * 17
86 = 2 * 43
87 = 3 * 29
91 = 7 * 13
93 = 3 * 31
94 = 2 * 47
95 = 5 * 19

## Semiprime  Classes Mod 2

For C=2 we have  classes 0 <= i < c-1, or simply class[0] and class[1].
Without constraints on N, 

class[0 mod 2]:
	4 = 2 * 2, 6 = 2 * 3, 10 = 2 * 5, 14 = 2 * 7, 22 = 2 * 11, 26 = 2 * 13, 34 = 2 * 17, 38 = 2 * 19, 46 = 2 * 23, 58 = 2 * 29, 62 = 2 * 31, 74 = 2 * 37, 82 = 2 * 41, 86 = 2 * 43, 94 = 2 * 47
class[1 mod 2]:
	9 = 3 * 3, 15 = 3 * 5, 21 = 3 * 7, 25 = 5 * 5, 33 = 3 * 11, 35 = 5 * 7, 39 = 3 * 13, 49 = 7 * 7, 51 = 3 * 17, 55 = 5 * 11, 57 = 3 * 19, 65 = 5 * 13, 69 = 3 * 23, 77 = 7 * 11, 85 = 5 * 17, 87 = 3 * 29, 91 = 7 * 13, 93 = 3 * 31, 95 = 5 * 19

We note trivially that without any constrains the class 0 mod 2 contains semiprimes that include factors for all primes and the class 1 mod 2 contains all primes except 2.

Therefore, without constraint we have C for N mod 2
C[0] = 0, 1
C[1] = 1

We have already provided proof that any number 0 mod 2 has a prime factor of 2, and that a semiprime is = 0 mod 2, n/2 is a prime.

Lemma: N is a semiprime can only be in the class 1 mod 2 if it has two prime factors that are 1 mod 2, or stated another way the product of two odd primes.

Proof. All primes except 2 are prime. Since 2 is the only even prime and any number multiplied by an even number results in an even number, if p or q is two in the semiprime n, then N must be even.

Now we can introduce a constraint that when considering semiprimes two we need only consider N = p * q where q are odd primes.

More generally, in terms of  classes outside of two, we will expand the constraint that N is coprime to C.

Therefore, with the constraint that N is a semiprime that is the product of two odd primes our classes are reduced

class[0 mod 2]:
	[empty set]
class[1 mod 2]:
    9 = 3 * 3, 15 = 3 * 5, 21 = 3 * 7, 25 = 5 * 5, 33 = 3 * 11, 35 = 5 * 7, 39 = 3 * 13, 49 = 7 * 7, 51 = 3 * 17, 55 = 5 * 11, 57 = 3 * 19, 65 = 5 * 13, 69 = 3 * 23, 77 = 7 * 11, 85 = 5 * 17, 87 = 3 * 29, 91 = 7 * 13, 93 = 3 * 31, 95 = 5 * 19
    
We Therefore, without constraint we have C for N mod 2
C[0] = {empty}
C[1] = 1

So we have proven, for a any semiprime n = 1 mod 2 with n = p*q, then p=1 mod 2 and q=1 mod 2.  For a any semiprime n = 0 mod 2 with n = p*q, then either p=0 mod 2 or q=0 modd 2.

## Semiprime  Classes Mod 3

For C=3 we have  classes 0 <= i < c-1, or simply class[0], class[1], and class[2] mod 3 respectively.

Without constraints on N:

class[0 mod 3]:
	6 = 2 * 3, 9 = 3 * 3, 15 = 3 * 5, 21 = 3 * 7, 33 = 3 * 11, 39 = 3 * 13, 51 = 3 * 17, 57 = 3 * 19, 69 = 3 * 23, 87 = 3 * 29, 93 = 3 * 31
class[1 mod 3]:
	4 = 2 * 2, 10 = 2 * 5, 22 = 2 * 11, 25 = 5 * 5, 34 = 2 * 17, 46 = 2 * 23, 49 = 7 * 7, 55 = 5 * 11, 58 = 2 * 29, 82 = 2 * 41, 85 = 5 * 17, 91 = 7 * 13, 94 = 2 * 47
class[2 mod 3]:
	14 = 2 * 7, 26 = 2 * 13, 35 = 5 * 7, 38 = 2 * 19, 62 = 2 * 31, 65 = 5 * 13, 74 = 2 * 37, 77 = 7 * 11, 86 = 2 * 43, 95 = 5 * 19

As with C = 2, the class 0 mod 3 can only contain semiprimes N = p * q, where p or q = 3.

We for N in the class 1 mod 3, p = 1 mod 3 and q = 1 mod 3, or p = 2 mod 3 and q = 2 mod 3.
We for N in the class 2 mod 3, p = 1 mod 3 and q = 2 mod 3, or p = 2 mod 3 and q = 1 mod 3.
 
Classes with constraint.
class[0 mod 3]:
	{empty set}
class[1 mod 3]:
	4 = 2 * 2, 10 = 2 * 5, 22 = 2 * 11, 25 = 5 * 5, 34 = 2 * 17, 46 = 2 * 23, 49 = 7 * 7, 55 = 5 * 11, 58 = 2 * 29, 82 = 2 * 41, 85 = 5 * 17, 91 = 7 * 13, 94 = 2 * 47
class[2 mod 3]:
	14 = 2 * 7, 26 = 2 * 13, 35 = 5 * 7, 38 = 2 * 19, 62 = 2 * 31, 65 = 5 * 13, 74 = 2 * 37, 77 = 7 * 11, 86 = 2 * 43, 95 = 5 * 19

## Semiprime  Classes Mod 4

For C=4 we have  classes 0 <= i < c-1, or simply class[0], class[1], class[2], and class[3] mod 4 respectively.

Classes without constraint.
class[0 mod 4]:
	4 = 2 * 2
class[1 mod 4]:
	9 = 3 * 3, 21 = 3 * 7, 25 = 5 * 5, 33 = 3 * 11, 49 = 7 * 7, 57 = 3 * 19, 65 = 5 * 13, 69 = 3 * 23, 77 = 7 * 11, 85 = 5 * 17, 93 = 3 * 31
class[2 mod 4]:
	6 = 2 * 3, 10 = 2 * 5, 14 = 2 * 7, 22 = 2 * 11, 26 = 2 * 13, 34 = 2 * 17, 38 = 2 * 19, 46 = 2 * 23, 58 = 2 * 29, 62 = 2 * 31, 74 = 2 * 37, 82 = 2 * 41, 86 = 2 * 43, 94 = 2 * 47
class[3 mod 4]:
	15 = 3 * 5, 35 = 5 * 7, 39 = 3 * 13, 51 = 3 * 17, 55 = 5 * 11, 87 = 3 * 29, 91 = 7 * 13, 95 = 5 * 19
	
We note, as given in our proofs, the class 0 mod 4 contains the single semiprime 4 = 2 * 2 and class 2 mod 2 contains only semiprimes where p or q = 2.

This leaves two classes where semiprimes exist mod 4, which is 1 mod 4 or 3 mod 4.

In the class N = 1 mod 4, we have p = 1 mod 4 * q = 1 mod 4, or p = 3 mod 4 and q = 3 mod 4.
In the class N = 3 mod 4, we have p = 1 mod 4 * q = 4 mod 4, or p = 3 mod 4 and q = 1 mod 4.

Finally, we show the classes mod 4 with the constraint that N is Coprime to C, with coprime meaning Gcd(N, C)=1
class[0 mod 4]:
	{{empty set}}
class[1 mod 4]:
	9 = 3 * 3, 21 = 3 * 7, 25 = 5 * 5, 33 = 3 * 11, 49 = 7 * 7, 57 = 3 * 19, 65 = 5 * 13, 69 = 3 * 23, 77 = 7 * 11, 85 = 5 * 17, 93 = 3 * 31
class[2 mod 4]:
	{{empty set}}
class[3 mod 4]:
	15 = 3 * 5, 35 = 5 * 7, 39 = 3 * 13, 51 = 3 * 17, 55 = 5 * 11, 87 = 3 * 29, 91 = 7 * 13, 95 = 5 * 19
	
## Semiprime  Classes Mod 6

For C=6 we have  classes 0 <= i < c-1, or simply class[0], class[1], class[2], class[3], class[4], class[5] mod 6 respectively.

Classes without constraint.
class[0 mod 6]:
	6 = 2 * 3
class[1 mod 6]:
	25 = 5 * 5, 49 = 7 * 7, 55 = 5 * 11, 85 = 5 * 17, 91 = 7 * 13
class[2 mod 6]:
	14 = 2 * 7, 26 = 2 * 13, 38 = 2 * 19, 62 = 2 * 31, 74 = 2 * 37, 86 = 2 * 43
class[3 mod 6]:
	9 = 3 * 3, 15 = 3 * 5, 21 = 3 * 7, 33 = 3 * 11, 39 = 3 * 13, 51 = 3 * 17, 57 = 3 * 19, 69 = 3 * 23, 87 = 3 * 29, 93 = 3 * 31
class[4 mod 6]:
	4 = 2 * 2, 10 = 2 * 5, 22 = 2 * 11, 34 = 2 * 17, 46 = 2 * 23, 58 = 2 * 29, 82 = 2 * 41, 94 = 2 * 47
class[5 mod 6]:
	35 = 5 * 7, 65 = 5 * 13, 77 = 7 * 11, 95 = 5 * 19
	
We note:
 - The class 0 mod 6 contains the single semiprime 6 = 2 * 3
 - The class 2 mod 6 contains only semiprimes where either p or q = 2, but not both.
 - The class 3 mod 6 contains only semiprimes where either p or q = 3.
 - The class 4 mod 6 contains only semiprimes where p or q = 2, which includes 2 * 2.
 
 
This leaves two classes where semiprimes exist mod 6:
 - 1 mod 6
 - 5 mod 6.

In the class N = 1 mod 6, the factors p and q are both in class 1 or class 5.
In the class N = 5 mod 4, the factors p and q each must be in class 1 or class 5 repspectively. 

Finally, we show the classes mod 6 with the constraint that N is Coprime to C, with coprime meaning Gcd(N, C)=1

Classes with constraint.
class[0 mod 6]:
	{{empty set}}
class[1 mod 6]:
	25 = 5 * 5, 49 = 7 * 7, 55 = 5 * 11, 85 = 5 * 17, 91 = 7 * 13
class[2 mod 6]:
	{{empty set}}
class[3 mod 6]:
	{{empty set}}
class[4 mod 6]:
	{{empty set}}
class[5 mod 6]:
	35 = 5 * 7, 65 = 5 * 13, 77 = 7 * 11, 95 = 5 * 19
	
## Semiprime  Classes Mod 9

For C=9 we have  classes 0 <= i < c-1, or simply class[0] to  class88] mod 9 respectively.

Classes without constraint.
	class[0 mod 9]:
		9 = 3 * 3
	class[1 mod 9]:
		10 = 2 * 5, 46 = 2 * 23, 55 = 5 * 11, 82 = 2 * 41, 91 = 7 * 13
	class[2 mod 9]:
		38 = 2 * 19, 65 = 5 * 13, 74 = 2 * 37
	class[3 mod 9]:
		21 = 3 * 7, 39 = 3 * 13, 57 = 3 * 19, 93 = 3 * 31
	class[4 mod 9]:
		4 = 2 * 2, 22 = 2 * 11, 49 = 7 * 7, 58 = 2 * 29, 85 = 5 * 17, 94 = 2 * 47
	class[5 mod 9]:
		14 = 2 * 7, 77 = 7 * 11, 86 = 2 * 43, 95 = 5 * 19
	class[6 mod 9]:
		6 = 2 * 3, 15 = 3 * 5, 33 = 3 * 11, 51 = 3 * 17, 69 = 3 * 23, 87 = 3 * 29
	class[7 mod 9]:
		25 = 5 * 5, 34 = 2 * 17
	class[8 mod 9]:
		26 = 2 * 13, 35 = 5 * 7, 62 = 2 * 31
		
We note:
 - The class 0 mod 9 contains the single semiprime 9 = 3 * 3 
 - The class 3 mod 9 contains only semiprimes where either p or q = 3, but not both.
 - The class 6 mod 9 contains only semiprimes where either p or q = 3.
 - The the remaingin classes 1, 2, 4, 5, 7 and 8 contain semiprimes where p or q !=3.
 
 
This leaves six classes where semiprimes exist mod 9:
 - 1 mod 9
 - 2 mod 9
 - 4 mod 9.
 - 5 mod 9.
 - 7 mod 9.
 - 8 mod 9.
 
The class of N is calcuated as (p%3 * q%3) % 9 which gives when 2 is not exclude as a constraint allows for semiprime factors to exist in all classes.




The classes mod 9 with the constraint that N is Coprime to C, with coprime meaning Gcd(N, C)=1

    class[0 mod 9]:
    	{{empty set}}
    class[1 mod 9]:
    	10 = 2 * 5, 46 = 2 * 23, 55 = 5 * 11, 82 = 2 * 41, 91 = 7 * 13
    class[2 mod 9]:
    	38 = 2 * 19, 65 = 5 * 13, 74 = 2 * 37
    class[3 mod 9]:
    	{{empty set}}
    class[4 mod 9]:
    	4 = 2 * 2, 22 = 2 * 11, 49 = 7 * 7, 58 = 2 * 29, 85 = 5 * 17, 94 = 2 * 47
    class[5 mod 9]:
    	14 = 2 * 7, 77 = 7 * 11, 86 = 2 * 43, 95 = 5 * 19
    class[6 mod 9]:
    	{{empty set}}
    class[7 mod 9]:
    	25 = 5 * 5, 34 = 2 * 17
    class[8 mod 9]:
    	26 = 2 * 13, 35 = 5 * 7, 62 = 2 * 31
		
		
## Semiprime  Classes Mod 36

For C=36 we have  classes 0 <= i < c-1, or simply class[0] to  class88] mod 9 respectively.

We explore 36 as while 18 allows us to put a constrain of 2 the C=9, it would then not include 2*2.

Using 36, we explore the all constraints of combination of 2, 3, 2*2 and 2*3, and 3*3	


Classes without constraint.
    class[0 mod 36]:
    	{{empty set}}
    class[1 mod 36]:
    	145 = 5 * 29, 217 = 7 * 31, 253 = 11 * 23, 289 = 17 * 17, 361 = 19 * 19, 469 = 7 * 67, 505 = 5 * 101, 649 = 11 * 59, 685 = 5 * 137, 721 = 7 * 103, 793 = 13 * 61, 865 = 5 * 173, 901 = 17 * 53, 973 = 7 * 139
    class[2 mod 36]:
    	38 = 2 * 19, 74 = 2 * 37, 146 = 2 * 73, 218 = 2 * 109, 254 = 2 * 127, 326 = 2 * 163, 362 = 2 * 181, 398 = 2 * 199, 542 = 2 * 271, 614 = 2 * 307, 758 = 2 * 379, 794 = 2 * 397, 866 = 2 * 433, 974 = 2 * 487
    class[3 mod 36]:
    	39 = 3 * 13, 111 = 3 * 37, 183 = 3 * 61, 219 = 3 * 73, 291 = 3 * 97, 327 = 3 * 109, 471 = 3 * 157, 543 = 3 * 181, 579 = 3 * 193, 687 = 3 * 229, 723 = 3 * 241, 831 = 3 * 277, 939 = 3 * 313
    class[4 mod 36]:
    	4 = 2 * 2
    class[5 mod 36]:
    	77 = 7 * 11, 185 = 5 * 37, 221 = 13 * 17, 329 = 7 * 47, 365 = 5 * 73, 437 = 19 * 23, 473 = 11 * 43, 545 = 5 * 109, 581 = 7 * 83, 689 = 13 * 53, 869 = 11 * 79, 905 = 5 * 181
    class[6 mod 36]:
    	6 = 2 * 3
    class[7 mod 36]:
    	115 = 5 * 23, 187 = 11 * 17, 259 = 7 * 37, 295 = 5 * 59, 403 = 13 * 31, 511 = 7 * 73, 583 = 11 * 53, 655 = 5 * 131, 763 = 7 * 109, 799 = 17 * 47, 835 = 5 * 167, 871 = 13 * 67, 943 = 23 * 41, 979 = 11 * 89
    class[8 mod 36]:
    	{{empty set}}
    class[9 mod 36]:
    	9 = 3 * 3
    class[10 mod 36]:
    	10 = 2 * 5, 46 = 2 * 23, 82 = 2 * 41, 118 = 2 * 59, 226 = 2 * 113, 262 = 2 * 131, 298 = 2 * 149, 334 = 2 * 167, 478 = 2 * 239, 514 = 2 * 257, 586 = 2 * 293, 622 = 2 * 311, 694 = 2 * 347, 766 = 2 * 383, 802 = 2 * 401, 838 = 2 * 419, 982 = 2 * 491
    class[11 mod 36]:
    	119 = 7 * 17, 155 = 5 * 31, 299 = 13 * 23, 335 = 5 * 67, 371 = 7 * 53, 407 = 11 * 37, 515 = 5 * 103, 551 = 19 * 29, 623 = 7 * 89, 695 = 5 * 139, 731 = 17 * 43, 767 = 13 * 59, 803 = 11 * 73
    class[12 mod 36]:
    	{{empty set}}
    class[13 mod 36]:
    	49 = 7 * 7, 85 = 5 * 17, 121 = 11 * 11, 265 = 5 * 53, 301 = 7 * 43, 445 = 5 * 89, 481 = 13 * 37, 517 = 11 * 47, 553 = 7 * 79, 589 = 19 * 31, 697 = 17 * 41, 841 = 29 * 29, 913 = 11 * 83, 949 = 13 * 73, 985 = 5 * 197
    class[14 mod 36]:
    	14 = 2 * 7, 86 = 2 * 43, 122 = 2 * 61, 158 = 2 * 79, 194 = 2 * 97, 302 = 2 * 151, 446 = 2 * 223, 482 = 2 * 241, 554 = 2 * 277, 626 = 2 * 313, 662 = 2 * 331, 698 = 2 * 349, 734 = 2 * 367, 842 = 2 * 421, 878 = 2 * 439, 914 = 2 * 457
    class[15 mod 36]:
    	15 = 3 * 5, 51 = 3 * 17, 87 = 3 * 29, 123 = 3 * 41, 159 = 3 * 53, 267 = 3 * 89, 303 = 3 * 101, 339 = 3 * 113, 411 = 3 * 137, 447 = 3 * 149, 519 = 3 * 173, 591 = 3 * 197, 699 = 3 * 233, 771 = 3 * 257, 807 = 3 * 269, 843 = 3 * 281, 879 = 3 * 293, 951 = 3 * 317
    class[16 mod 36]:
    	{{empty set}}
    class[17 mod 36]:
    	161 = 7 * 23, 305 = 5 * 61, 341 = 11 * 31, 377 = 13 * 29, 413 = 7 * 59, 485 = 5 * 97, 629 = 17 * 37, 737 = 11 * 67, 917 = 7 * 131, 989 = 23 * 43
    class[18 mod 36]:
    	{{empty set}}
    class[19 mod 36]:
    	55 = 5 * 11, 91 = 7 * 13, 235 = 5 * 47, 415 = 5 * 83, 451 = 11 * 41, 559 = 13 * 43, 667 = 23 * 29, 703 = 19 * 37, 955 = 5 * 191
    class[20 mod 36]:
    	{{empty set}}
    class[21 mod 36]:
    	21 = 3 * 7, 57 = 3 * 19, 93 = 3 * 31, 129 = 3 * 43, 201 = 3 * 67, 237 = 3 * 79, 309 = 3 * 103, 381 = 3 * 127, 417 = 3 * 139, 453 = 3 * 151, 489 = 3 * 163, 597 = 3 * 199, 633 = 3 * 211, 669 = 3 * 223, 813 = 3 * 271, 849 = 3 * 283, 921 = 3 * 307, 993 = 3 * 331
    class[22 mod 36]:
    	22 = 2 * 11, 58 = 2 * 29, 94 = 2 * 47, 166 = 2 * 83, 202 = 2 * 101, 274 = 2 * 137, 346 = 2 * 173, 382 = 2 * 191, 454 = 2 * 227, 526 = 2 * 263, 562 = 2 * 281, 634 = 2 * 317, 706 = 2 * 353, 778 = 2 * 389, 886 = 2 * 443, 922 = 2 * 461, 958 = 2 * 479
    class[23 mod 36]:
    	95 = 5 * 19, 203 = 7 * 29, 527 = 17 * 31, 635 = 5 * 127, 671 = 11 * 61, 707 = 7 * 101, 779 = 19 * 41, 815 = 5 * 163, 851 = 23 * 37, 923 = 13 * 71, 959 = 7 * 137, 995 = 5 * 199
    class[24 mod 36]:
    	{{empty set}}
    class[25 mod 36]:
    	25 = 5 * 5, 133 = 7 * 19, 169 = 13 * 13, 205 = 5 * 41, 493 = 17 * 29, 529 = 23 * 23, 565 = 5 * 113, 745 = 5 * 149, 781 = 11 * 71, 817 = 19 * 43, 889 = 7 * 127, 961 = 31 * 31
    class[26 mod 36]:
    	26 = 2 * 13, 62 = 2 * 31, 134 = 2 * 67, 206 = 2 * 103, 278 = 2 * 139, 314 = 2 * 157, 386 = 2 * 193, 422 = 2 * 211, 458 = 2 * 229, 566 = 2 * 283, 674 = 2 * 337, 746 = 2 * 373, 818 = 2 * 409, 926 = 2 * 463, 998 = 2 * 499
    class[27 mod 36]:
    	{{empty set}}
    class[28 mod 36]:
    	{{empty set}}
    class[29 mod 36]:
    	65 = 5 * 13, 209 = 11 * 19, 497 = 7 * 71, 533 = 13 * 41, 713 = 23 * 31, 749 = 7 * 107, 785 = 5 * 157, 893 = 19 * 47, 965 = 5 * 193
    class[30 mod 36]:
    	{{empty set}}
    class[31 mod 36]:
    	247 = 13 * 19, 319 = 11 * 29, 355 = 5 * 71, 391 = 17 * 23, 427 = 7 * 61, 535 = 5 * 107, 679 = 7 * 97, 895 = 5 * 179
    class[32 mod 36]:
    	{{empty set}}
    class[33 mod 36]:
    	33 = 3 * 11, 69 = 3 * 23, 141 = 3 * 47, 177 = 3 * 59, 213 = 3 * 71, 249 = 3 * 83, 321 = 3 * 107, 393 = 3 * 131, 501 = 3 * 167, 537 = 3 * 179, 573 = 3 * 191, 681 = 3 * 227, 717 = 3 * 239, 753 = 3 * 251, 789 = 3 * 263, 933 = 3 * 311
    class[34 mod 36]:
    	34 = 2 * 17, 106 = 2 * 53, 142 = 2 * 71, 178 = 2 * 89, 214 = 2 * 107, 358 = 2 * 179, 394 = 2 * 197, 466 = 2 * 233, 502 = 2 * 251, 538 = 2 * 269, 718 = 2 * 359, 862 = 2 * 431, 898 = 2 * 449, 934 = 2 * 467
    class[35 mod 36]:
    	35 = 5 * 7, 143 = 11 * 13, 215 = 5 * 43, 287 = 7 * 41, 323 = 17 * 19, 395 = 5 * 79, 611 = 13 * 47, 755 = 5 * 151, 791 = 7 * 113, 899 = 29 * 31
    
Classes with constraint.
    class[0 mod 36]:
    	{{empty set}}
    class[1 mod 36]:
    	145 = 5 * 29, 217 = 7 * 31, 253 = 11 * 23, 289 = 17 * 17, 361 = 19 * 19, 469 = 7 * 67, 505 = 5 * 101, 649 = 11 * 59, 685 = 5 * 137, 721 = 7 * 103, 793 = 13 * 61, 865 = 5 * 173, 901 = 17 * 53, 973 = 7 * 139
    class[2 mod 36]:
    	{{empty set}}
    class[3 mod 36]:
    	{{empty set}}
    class[4 mod 36]:
    	{{empty set}}
    class[5 mod 36]:
    	77 = 7 * 11, 185 = 5 * 37, 221 = 13 * 17, 329 = 7 * 47, 365 = 5 * 73, 437 = 19 * 23, 473 = 11 * 43, 545 = 5 * 109, 581 = 7 * 83, 689 = 13 * 53, 869 = 11 * 79, 905 = 5 * 181
    class[6 mod 36]:
    	{{empty set}}
    class[7 mod 36]:
    	115 = 5 * 23, 187 = 11 * 17, 259 = 7 * 37, 295 = 5 * 59, 403 = 13 * 31, 511 = 7 * 73, 583 = 11 * 53, 655 = 5 * 131, 763 = 7 * 109, 799 = 17 * 47, 835 = 5 * 167, 871 = 13 * 67, 943 = 23 * 41, 979 = 11 * 89
    class[8 mod 36]:
    	{{empty set}}
    class[9 mod 36]:
    	{{empty set}}
    class[10 mod 36]:
    	{{empty set}}
    class[11 mod 36]:
    	119 = 7 * 17, 155 = 5 * 31, 299 = 13 * 23, 335 = 5 * 67, 371 = 7 * 53, 407 = 11 * 37, 515 = 5 * 103, 551 = 19 * 29, 623 = 7 * 89, 695 = 5 * 139, 731 = 17 * 43, 767 = 13 * 59, 803 = 11 * 73
    class[12 mod 36]:
    	{{empty set}}
    class[13 mod 36]:
    	49 = 7 * 7, 85 = 5 * 17, 121 = 11 * 11, 265 = 5 * 53, 301 = 7 * 43, 445 = 5 * 89, 481 = 13 * 37, 517 = 11 * 47, 553 = 7 * 79, 589 = 19 * 31, 697 = 17 * 41, 841 = 29 * 29, 913 = 11 * 83, 949 = 13 * 73, 985 = 5 * 197
    class[14 mod 36]:
    	{{empty set}}
    class[15 mod 36]:
    	{{empty set}}
    class[16 mod 36]:
    	{{empty set}}
    class[17 mod 36]:
    	161 = 7 * 23, 305 = 5 * 61, 341 = 11 * 31, 377 = 13 * 29, 413 = 7 * 59, 485 = 5 * 97, 629 = 17 * 37, 737 = 11 * 67, 917 = 7 * 131, 989 = 23 * 43
    class[18 mod 36]:
    	{{empty set}}
    class[19 mod 36]:
    	55 = 5 * 11, 91 = 7 * 13, 235 = 5 * 47, 415 = 5 * 83, 451 = 11 * 41, 559 = 13 * 43, 667 = 23 * 29, 703 = 19 * 37, 955 = 5 * 191
    class[20 mod 36]:
    	{{empty set}}
    class[21 mod 36]:
    	{{empty set}}
    class[22 mod 36]:
    	{{empty set}}
    class[23 mod 36]:
    	95 = 5 * 19, 203 = 7 * 29, 527 = 17 * 31, 635 = 5 * 127, 671 = 11 * 61, 707 = 7 * 101, 779 = 19 * 41, 815 = 5 * 163, 851 = 23 * 37, 923 = 13 * 71, 959 = 7 * 137, 995 = 5 * 199
    class[24 mod 36]:
    	{{empty set}}
    class[25 mod 36]:
    	25 = 5 * 5, 133 = 7 * 19, 169 = 13 * 13, 205 = 5 * 41, 493 = 17 * 29, 529 = 23 * 23, 565 = 5 * 113, 745 = 5 * 149, 781 = 11 * 71, 817 = 19 * 43, 889 = 7 * 127, 961 = 31 * 31
    class[26 mod 36]:
    	{{empty set}}
    class[27 mod 36]:
    	{{empty set}}
    class[28 mod 36]:
    	{{empty set}}
    class[29 mod 36]:
    	65 = 5 * 13, 209 = 11 * 19, 497 = 7 * 71, 533 = 13 * 41, 713 = 23 * 31, 749 = 7 * 107, 785 = 5 * 157, 893 = 19 * 47, 965 = 5 * 193
    class[30 mod 36]:
    	{{empty set}}
    class[31 mod 36]:
    	247 = 13 * 19, 319 = 11 * 29, 355 = 5 * 71, 391 = 17 * 23, 427 = 7 * 61, 535 = 5 * 107, 679 = 7 * 97, 895 = 5 * 179
    class[32 mod 36]:
    	{{empty set}}
    class[33 mod 36]:
    	{{empty set}}
    class[34 mod 36]:
    	{{empty set}}
    class[35 mod 36]:
    	35 = 5 * 7, 143 = 11 * 13, 215 = 5 * 43, 287 = 7 * 41, 323 = 17 * 19, 395 = 5 * 79, 611 = 13 * 47, 755 = 5 * 151, 791 = 7 * 113, 899 = 29 * 31

The non-empty set includes c +/1 mod 6
    Classes with constraint.
    class[1 mod 36]:
    	145 = 5 * 29, 217 = 7 * 31, 253 = 11 * 23, 289 = 17 * 17, 361 = 19 * 19, 469 = 7 * 67, 505 = 5 * 101, 649 = 11 * 59, 685 = 5 * 137, 721 = 7 * 103, 793 = 13 * 61, 865 = 5 * 173, 901 = 17 * 53, 973 = 7 * 139
    class[5 mod 36]:
    	77 = 7 * 11, 185 = 5 * 37, 221 = 13 * 17, 329 = 7 * 47, 365 = 5 * 73, 437 = 19 * 23, 473 = 11 * 43, 545 = 5 * 109, 581 = 7 * 83, 689 = 13 * 53, 869 = 11 * 79, 905 = 5 * 181
    class[7 mod 36]:
    	115 = 5 * 23, 187 = 11 * 17, 259 = 7 * 37, 295 = 5 * 59, 403 = 13 * 31, 511 = 7 * 73, 583 = 11 * 53, 655 = 5 * 131, 763 = 7 * 109, 799 = 17 * 47, 835 = 5 * 167, 871 = 13 * 67, 943 = 23 * 41, 979 = 11 * 89
    class[11 mod 36]:
    	119 = 7 * 17, 155 = 5 * 31, 299 = 13 * 23, 335 = 5 * 67, 371 = 7 * 53, 407 = 11 * 37, 515 = 5 * 103, 551 = 19 * 29, 623 = 7 * 89, 695 = 5 * 139, 731 = 17 * 43, 767 = 13 * 59, 803 = 11 * 73
    class[13 mod 36]:
    	49 = 7 * 7, 85 = 5 * 17, 121 = 11 * 11, 265 = 5 * 53, 301 = 7 * 43, 445 = 5 * 89, 481 = 13 * 37, 517 = 11 * 47, 553 = 7 * 79, 589 = 19 * 31, 697 = 17 * 41, 841 = 29 * 29, 913 = 11 * 83, 949 = 13 * 73, 985 = 5 * 197
    class[17 mod 36]:
    	161 = 7 * 23, 305 = 5 * 61, 341 = 11 * 31, 377 = 13 * 29, 413 = 7 * 59, 485 = 5 * 97, 629 = 17 * 37, 737 = 11 * 67, 917 = 7 * 131, 989 = 23 * 43
    class[19 mod 36]:
    	55 = 5 * 11, 91 = 7 * 13, 235 = 5 * 47, 415 = 5 * 83, 451 = 11 * 41, 559 = 13 * 43, 667 = 23 * 29, 703 = 19 * 37, 955 = 5 * 191
    class[23 mod 36]:
    	95 = 5 * 19, 203 = 7 * 29, 527 = 17 * 31, 635 = 5 * 127, 671 = 11 * 61, 707 = 7 * 101, 779 = 19 * 41, 815 = 5 * 163, 851 = 23 * 37, 923 = 13 * 71, 959 = 7 * 137, 995 = 5 * 199
    class[25 mod 36]:
    	25 = 5 * 5, 133 = 7 * 19, 169 = 13 * 13, 205 = 5 * 41, 493 = 17 * 29, 529 = 23 * 23, 565 = 5 * 113, 745 = 5 * 149, 781 = 11 * 71, 817 = 19 * 43, 889 = 7 * 127, 961 = 31 * 31
    class[29 mod 36]:
    	65 = 5 * 13, 209 = 11 * 19, 497 = 7 * 71, 533 = 13 * 41, 713 = 23 * 31, 749 = 7 * 107, 785 = 5 * 157, 893 = 19 * 47, 965 = 5 * 193
    class[31 mod 36]:
    	247 = 13 * 19, 319 = 11 * 29, 355 = 5 * 71, 391 = 17 * 23, 427 = 7 * 61, 535 = 5 * 107, 679 = 7 * 97, 895 = 5 * 179
    class[35 mod 36]:
    	35 = 5 * 7, 143 = 11 * 13, 215 = 5 * 43, 287 = 7 * 41, 323 = 17 * 19, 395 = 5 * 79, 611 = 13 * 47, 755 = 5 * 151, 791 = 7 * 113, 899 = 29 * 31
		
We note the s in class 1 and the deltas between each  are a muliple of c:
class[1 mod 36]:
    	145, 217, 253, 289, 361, 469, 505, 649, 685, 721, 793, 865, 901, 973
		
|N	|Delta	|Delta/c	|Equation
| ------------ | ------------ | ------------ | ------------ |
145	|	|	|	|
|217	|72	|2	|217 = 145 + (36 * 2)|
|253	|36	|1	|253 = 217 + (36 * 1)|
|289	|36	|1	|289 = 253 + (36 * 1)|
|361	|72	|2	|361 = 289 + (36 * 2)|
|469	|108	|3	|469 = 361 + (36 * 3)|
|505	|36	|1	|505 = 469 + (36 * 1)|
|649	|144	|4	|649 = 505 + (36 * 4)|
|685	|36	|1	|685 = 649 + (36 * 1)|
|721	|36	|1	|721 = 685 + (36 * 1)|
|793	|72	|2	|793 = 721 + (36 * 2)|
|865	|72	|2	|865 = 793 + (36 * 2)|
|901	|36	|1	|901 = 865 + (36 * 1)|
|973	|72	|2	|973 = 901 + (36 * 2)|		
		
The same property holds for all other classes.

For example in class 5 we have:

    class[5 mod 36]:
    	77  + 36*3 = 185  + 36*1 = 221 ...		
And		
   class[7 mod 36]:
    	115 + 36*2 = 187  + 26*2 = 259  + 36 = 295...

his leads us to a linear equation for semiprimes in a  class.


# combining classes:

suppose we wish to factor n = 57261023

It's prime factorization is 5347*10709 but is unknown to us.

n % 3 = 2
	this means in C[3]:
		p = 2 and q = 1, or p = 1 and q = 2
n % 4 = 3 
	this means in C[4]:
		p = 1 and q = 3, or p = 3 and q = 1
n % 6 = 5
	this means in C[6]
		p = 1 and q = 5, or p = 1 and q = 5
n % 9 = 8 *
	this means in C[9]
		p = 1 and q = 8, or p = 8 and q = 1; or,
		p = 2 and q = 4, or q = 4 and p = 2; or,
		p = 5 and q = 7, or q = 5 and p = 7;
n % 10 = 3 **
	this means in C[10]
		p = 1 and q = 3, or p = 3 and q = 1; or,
		p = 9 and q = 7, or p = 7 and q = 9

n % 36 = 35 ***
	this means in C[36]
		p = 1 and q = 35, or p = 35 and q = 1; or,
		p = 5 and q = 7, or p = 7 and q = 5; or,
		p = 11 and q = 13, or p = 13 and q = 11; or,
		p = 17 and q = 19, or p = 19 and q = 17; or,
		p = 23 and q = 25, or p = 25 and q = 23; or,
		p = 29 and q = 31, or p = 31 and q = 29; or,

Reviewing the given the constraints on p and q we find the following relations that satisfy C[10] and C[26]
	1:
	
		C[10] (n % 10 = 3)
			p = 1 and q = 3, or p = 3 and q = 1; or, 
		C[36] (n % 36 = 35)	
			p = 11 and q = 13, or p = 13 and q = 11; or,
	2:
		C[10] (n % 10 = 3)
			p = 9 and q = 7, or p = 7 and q = 9
		C[36] (n % 36 = 35)
			p = 17 and q = 19, or p = 19 and q = 17;

	Therefore, we can search reduce or search for p using the linear relationship p = 9 + 10x and p = 17 + 36y
	
		10709-17 = 10692
		10709-9  = 10700
			step size = 10*36 = 360
		29*360 = 10,440
		10709-10,440 = 269
			269 = 9 + 26 * 10
			269 = 17 + 7 * 36
        269 + 29 * 360 = 10,709

        Bruteforce:
            9 + 10x:    let x=1, then we have 9 + 10 = 19
            17 + 36x:   let x=1, then we have 17 + 36 = 53

            17 + 36x:   let x=2, then we have 17 + 36*2 = 89 == 9 + 10 * 8
                        so we start at 89, since gcd(10,36)=2, we step 180 and not 360.
                        and 89 + 180 * i = 9 mod 10 and 17 mod 16.

    3:
        But how do we choose, or must we test all relationships?
        If we add another class, let's say C[30] then we have n = 23 mod 30
        
        In C[30][23] we have:
          p = 1 and q = 23, or p = 23 and q = 1; or,
          p = 7 and q = 29, or p = 29 and q = 7; or,
          p = 11 and q = 13, or p = 13 and q = 11; or,
          p = 17 and q = 19, or p = 19 and q = 17; or,

        Conditions p=1 and p = 11, match our first relations while p = 7 and p = 17 match our second relationship of C[10] and [C36].
        However, the combination of 10, 36 and 30 gives us 10,800 as step size which overflows both factors of n, unless we were to start at -91.
            using the equation = 9+ 10x and p= 17 + 36x and p = 23 + 30x.
            10,800 mod 10 = 0 - 9 = -9
            10,800 mod 36 = 0 - 17 = -17
            10,800 mod 30 = 0 - 23 = -23
            -49 = -9 + -17 + -23
            10,800 + -49 = 10,751
            10,751 + -49 =  10,751 + -49 = 10,702

5347 % 36 = 19
10709 % 36 = 17

5347 % 10 = 7
10709 % 10 = 9

5347 % 30 = 7
10709 % 30 = 29


# Axioms

The Fundamental Theorem of Arithmetic can be stated as follows:

For any positive integer n > 1, there exist unique prime numbers p1, p2, ..., pk and positive integers e1, e2, ..., ek such that:

n = p1^e1 * p2^e2 * ... * pk^ek,

where p1 < p2 < ... < pk are prime numbers and e1, e2, ..., ek are positive integers.


	
** We did not exhuastively discuss p and q for C[9]
If n % 9 = 8 in base 9, we are looking for the potential least significant digits (p and q) that multiply to 8 mod 9.

In base 9, we can represent any number as a sum of its digits multiplied by powers of 9. Thus, if we consider the least significant digit of n, it can be represented as p * 9^0, where p is the digit.

To find the potential least significant digits that multiply to 8 mod 9, we need to solve the equation p * q ≡ 8 (mod 9), where p and q are digits.

Since we have constraints on C[9][3] and C[9][6], we know all multiples in those classes will contain a factor of 3, so we can exclude them.

This leaves use the non-empty sets, 1, 2, 4, 5, 7 and 8.

Enumerating them by brute-force in this case, we have the following possibilities:

	p = 1 and q = 8:
	1 * 8 ≡ 8 (mod 9)

	p = 2 and q = 4:
	2 * 4 ≡ 8 (mod 9)

	p = 4 and q = 2:
	4 * 2 ≡ 8 (mod 9)

	p = 5 and q = 7:
	5 * 7 ≡ 8 (mod 9)

	p = 7 and q = 5:
	7 * 5 ≡ 8 (mod 9)

	p = 8 and q = 1:
	8 * 1 ≡ 8 (mod 9)

Therefore, the potential least significant digits (p and q) that multiply to 8 mod 9 are p = 1 and q = 8, p = 2 and q = 4, p = 4 and q = 2, as well as p = 8 and q = 1.


** we have not discussed C[10] yet, or C[5], but for completeness before moving on:


If n % 10 = 3 in base 10, we are looking for the potential least significant digits (p and q) that multiply to 3 mod 10.

In base 10, we can represent any number as a sum of its digits multiplied by powers of 10. Thus, if we consider the least significant digit of n, it can be represented as p * 10^0, where p is the digit.

To find the potential least significant digits that multiply to 3 mod 10, we need to solve the equation p * q ≡ 3 (mod 10), where p and q are digits.

In this case, we have the following possibilities:

	p = 1 and q = 3:
	1 * 3 ≡ 3 (mod 10)

	p = 3 and q = 1:
	3 * 1 ≡ 3 (mod 10)

	p = 9 and q = 7:
	9 * 7 ≡ 3 (mod 10)

	p = 7 and q = 9:
	7 * 9 ≡ 3 (mod 10)

Therefore, the potential least significant digits (p and q) that multiply to 3 mod 10 are p = 1 and q = 3, or p = 3 and q = 1, as well as p = 9 and q = 7, or p = 7 and q = 9.

# Table

Mod 2: 