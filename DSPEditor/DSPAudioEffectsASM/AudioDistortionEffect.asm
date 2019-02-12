.data

max_value_dist real8 0.0
min_value_dist real8 0.0

max_value_dist_multiple real8 0.0
min_value_dist_multiple real8 0.0

multiplication real8 1.75

.code

distortion_init proc
	movsd max_value_dist, xmm0
	movsd min_value_dist, xmm1
	mulsd xmm0, multiplication
	mulsd xmm1, multiplication
	movsd max_value_dist_multiple, xmm0
	movsd min_value_dist_multiple, xmm1
	xorps xmm0, xmm0
	xorps xmm1, xmm1
	ret

distortion_init endp

distortion_process proc
	
	movsd xmm3, xmm0

@check_max_value:
	movsd xmm2, max_value_dist
	ucomisd xmm0, xmm2
	ja @set_max_value
	movsd xmm0, xmm3
	movsd xmm1, min_value_dist
	ucomisd xmm0, xmm1
	jb @set_min_value
	movsd xmm0, xmm3
	ret

@set_max_value:
	movsd xmm0, max_value_dist_multiple
	ret

@set_min_value:
	movsd xmm0, min_value_dist_multiple
	ret


distortion_process endp

end