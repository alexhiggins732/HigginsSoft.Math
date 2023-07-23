.code

BinaryMod PROC
    ; Arguments are passed in registers:
    ; a -> RCX
    ; b -> RDX

    mov rax, rcx

BinaryMod_Loop:
    cmp rax, rdx     	; Compare a and b
    jl BinaryMod_Ret  	; If a < b, exit the loop
    mov rcx, rdx     	; Set b1 = b
    mov r8, rcx			; Set t = b1
    shl r8, 1			; Set t = t << 1

BinaryMod_InnerLoop:
    test r8, r8			; Check if t > 0 and t < a
    jz BinaryMod_End  	; If t is zero, exit the inner loop
    cmp r8, rax
    jnl BinaryMod_End  	; If t >= a, exit the inner loop

    ; Set b1 = t
    mov rcx, r8		 	; Set b1 = t;
    shl r8, 1			; Set t = t << 1 

    jmp BinaryMod_InnerLoop

BinaryMod_End:
    sub rax, rcx		; Subtract b1 from a

	jmp BinaryMod_Loop
    ; Return a


BinaryMod_Ret:
    ret
BinaryMod ENDP

END
