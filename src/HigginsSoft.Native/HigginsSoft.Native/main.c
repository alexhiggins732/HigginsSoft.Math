// Declaration of the assembly function
#include <stdio.h>

extern int BinaryMod(int a, int b);

int main()
{
	int b = 10;
	int a = 3;
	int result = BinaryMod(b, a);

	printf("Result: %d mod %d = %d\n", b, a, result);

	b = 67;
	a = 17;
	result = BinaryMod(b, a);

	printf("Result: %d mod %d = %d\n", b, a, result);

	return 0;
}