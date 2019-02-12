.data

delay_miliseconds dword 0
delay_samples dword 0
decay real8 0.0

sample_rate real8 44.1
temp_value real8 0.0
offset_value sword 0

tab_length dword 0
tab_begin_index dword 0
tab_end_index dword 0

loop_counter dword 0
const_value real8 8.0

.code

reverb_init proc

	mov delay_miliseconds, ecx
	movsd decay, xmm1

	xorps xmm1, xmm1

	cvtsi2sd xmm1, ecx
	mulsd xmm1, sample_rate

	xor eax, eax
	cvttsd2si eax, xmm1
	mov delay_samples, eax

	ret

reverb_init endp

reverb_process proc

	mov eax, edx
	mov tab_begin_index, eax
	mov eax, r8d
	mov tab_end_index, eax

	mov eax, tab_end_index
	sub eax, delay_samples
	mov tab_length, eax

	mov eax, tab_begin_index
	mov loop_counter, eax

	mov eax, delay_samples
	cvtsi2sd xmm2, eax
	
	xor rax, rax
	xorps xmm0, xmm0
	xorps xmm1, xmm1

@processing_loop:

	mov eax, loop_counter
	
	movsd xmm0, qword ptr [rcx + rax * 8]
	addsd xmm0, xmm0
	movsd qword ptr [rcx + rax * 8], xmm0

	mulsd xmm0, decay

	cvtsi2sd xmm3, rax
	addsd xmm3, xmm2
	mulsd xmm3, const_value
	cvttsd2si rax, xmm3

	movsd xmm1, qword ptr [rcx + rax]
	addsd xmm1, xmm0
	movsd qword ptr [rcx + rax], xmm1

	add loop_counter, 1
	mov eax, loop_counter
	cmp eax, tab_length

	je @end
	jmp @processing_loop

@end:
	
	ret

reverb_process endp


end
