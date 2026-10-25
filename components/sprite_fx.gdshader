shader_type canvas_item;

// Outline + hit flash for AnimatedSprite2D. Driven by SpriteFxComponent, which
// feeds in the current frame's atlas region and flip state every frame.

uniform vec4 outline_color : source_color = vec4(1.0);
uniform float outline_width = 1.0; // in texture pixels
uniform vec4 flash_color : source_color = vec4(1.0);
uniform float flash : hint_range(0.0, 1.0) = 0.0;

uniform vec4 region = vec4(0.0, 0.0, 1.0, 1.0); // current frame in atlas UVs (min.xy, max.zw)
uniform vec2 flip = vec2(1.0); // -1 on an axis when the sprite is flipped

varying vec4 modulate;

void vertex() {
	modulate = COLOR;

	// Grow the quad so the outline has room even when the art touches the frame edge.
	vec2 dir = sign(VERTEX);
	VERTEX += dir * outline_width;
	UV += dir * flip * outline_width * TEXTURE_PIXEL_SIZE;
}

// Alpha at uv, treating anything outside the current frame as empty
// (otherwise the neighbouring frame in the sheet bleeds in).
float alpha_at(sampler2D tex, vec2 uv) {
	if (uv.x < region.x || uv.y < region.y || uv.x > region.z || uv.y > region.w) {
		return 0.0;
	}
	return texture(tex, uv).a;
}

void fragment() {
	vec4 tex = texture(TEXTURE, UV);
	tex.a = alpha_at(TEXTURE, UV);

	vec2 px = TEXTURE_PIXEL_SIZE * outline_width;
	float edge = max(
		max(alpha_at(TEXTURE, UV + vec2(px.x, 0.0)), alpha_at(TEXTURE, UV - vec2(px.x, 0.0))),
		max(alpha_at(TEXTURE, UV + vec2(0.0, px.y)), alpha_at(TEXTURE, UV - vec2(0.0, px.y))));

	vec4 body = vec4(mix(tex.rgb, flash_color.rgb, flash), tex.a);
	vec4 outline = vec4(outline_color.rgb, outline_color.a * edge);

	COLOR = mix(outline, body, body.a) * modulate;
}
