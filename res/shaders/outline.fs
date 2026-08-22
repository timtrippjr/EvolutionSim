#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

uniform vec2 texelSize;
uniform vec4 outlineColor;

out vec4 finalColor;

void main() {
    vec4 texColor = texture(texture0, fragTexCoord);
    
    if (texColor.a > 0.0) {
        finalColor = texColor * fragColor * colDiffuse;
    } else {
        // Sample 4 cardinal neighbors
        float alpha = 0.0;
        alpha += texture(texture0, fragTexCoord + vec2(texelSize.x, 0.0)).a;  // Right
        alpha += texture(texture0, fragTexCoord + vec2(-texelSize.x, 0.0)).a; // Left
        alpha += texture(texture0, fragTexCoord + vec2(0.0, texelSize.y)).a;  // Down
        alpha += texture(texture0, fragTexCoord + vec2(0.0, -texelSize.y)).a; // Up
        
        // If any neighbor has alpha, render the outline color
        if (alpha > 0.0) {
            finalColor = outlineColor;
        } else {
            finalColor = vec4(0.0);
        }
    }
}
