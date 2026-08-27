#version 330

// IMPLICITLY LISTENED TO BY RAYLIB!!
in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;
//

uniform vec2 texelSize;
uniform vec4 outlineColor;

out vec4 finalColor;

void main() {
    vec4 texColor = texture(texture0, fragTexCoord);
    
    if (texColor.a > 0.0) {
        finalColor = texColor * fragColor * colDiffuse;
    } else {
        float alpha = 0.0;
        alpha += texture(texture0, fragTexCoord + vec2(texelSize.x, 0.0)).a;
        alpha += texture(texture0, fragTexCoord + vec2(-texelSize.x, 0.0)).a;
        alpha += texture(texture0, fragTexCoord + vec2(0.0, texelSize.y)).a;
        alpha += texture(texture0, fragTexCoord + vec2(0.0, -texelSize.y)).a;
        
        if (alpha > 0.0) {
            finalColor = outlineColor;
        } else{
            finalColor = vec4(0.0);
        }
    }
}
