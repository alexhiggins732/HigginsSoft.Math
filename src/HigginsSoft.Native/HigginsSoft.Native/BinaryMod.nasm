; Assuming the method receives the 'a' parameter in rdi and the 'b' parameter in rsi
; Assuming the return value will be stored in eax

BinaryMod:
    xor eax, eax          ; Initialize eax to 0

BinaryMod_Loop:
    cmp rdi, rsi          ; Compare a with b
    jl BinaryMod_End     ; If a < b, exit the loop

    mov eax, rsi          ; Move b to eax
    mov ecx, eax          ; Move b to ecx
    shl ecx, 1            ; Multiply b by 2 and store the result in ecx

BinaryMod_InnerLoop:
    test ecx, ecx         ; Check if ecx is zero
    jz BinaryMod_End      ; If ecx is zero, exit the inner loop

    cmp ecx, rdi          ; Compare ecx with a
    jnl BinaryMod_End     ; If ecx >= a, exit the inner loop

    mov eax, ecx          ; Move ecx to eax
    mov ecx, eax          ; Move ecx to the temporary register ecx
    shl ecx, 1            ; Multiply ecx by 2

    jmp BinaryMod_InnerLoop

BinaryMod_End:
    sub rdi, eax          ; Subtract the accumulated b value from a
    mov eax, edi          ; Move the result back to eax

    ret                   ; Return from the function
