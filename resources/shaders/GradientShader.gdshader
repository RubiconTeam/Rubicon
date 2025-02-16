shader_type canvas_item;

uniform vec4 black_color:source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform vec4 white_color:source_color = vec4(1.0, 1.0, 1.0, 1.0);

// OG written by firubii
varying vec4 modulate;
void fragment()
{
	vec4 col = texture(TEXTURE, UV);
	vec4 colorLerp = mix(black_color, white_color, col);
	colorLerp.a = col.a;

	colorLerp *= modulate;

	COLOR = colorLerp;
}

void vertex()
{
	modulate = COLOR;
}