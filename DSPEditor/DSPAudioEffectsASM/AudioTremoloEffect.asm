.data

tremolo_depth real8 0.0
tremolo_counter_limit sword 0
tremolo_control sword 1
tremolo_mod sword 0
tremolo_offset real8 0.0

const_one real8 1.0
const_minus_one_short sword -1
const_one_short sword 1

temp_value real8 0.0

.code 

tremolo_init proc

	mov tremolo_counter_limit, cx	
	movsd tremolo_depth, xmm1

	movsd xmm1, const_one
	subsd xmm1, tremolo_depth
	movsd tremolo_offset, xmm1

	ret

tremolo_init endp

tremolo_process proc 

	
	movsd xmm2, xmm0

	movzx eax, tremolo_mod
	cvtsi2sd xmm0, eax
	movsd xmm1, tremolo_depth
	movsd temp_value, xmm1
	mulsd xmm0, temp_value
	movzx eax, tremolo_counter_limit
	cvtsi2sd xmm1, eax
	movsd temp_value, xmm1
	divsd xmm0, temp_value

	addsd xmm0, tremolo_offset

	movsd temp_value, xmm0
	mulsd xmm2, temp_value

	movsd xmm0, xmm2

	mov ax, tremolo_mod
	add ax, tremolo_control
	mov tremolo_mod, ax

	cmp ax, tremolo_counter_limit
	ja @reverse_tremolo_control
	cmp ax, 0
	je @averse_tremolo_control
	ret

@reverse_tremolo_control:
	mov ax, const_minus_one_short
	mov tremolo_control, ax
	ret

@averse_tremolo_control:
	mov ax, const_one_short
	mov tremolo_control, ax
	ret


tremolo_process endp

end