#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

uniform float waterLevel;
uniform float sandLevel;

uniform vec4 waterLight;
uniform vec4 waterDark;
uniform vec4 sandLight;
uniform vec4 sandDark;
uniform vec4 grassLight;
uniform vec4 grassDark;

out vec4 finalColor;

void main() {
    float height = texture(texture0, fragTexCoord).r;
    
    vec4 terrainColor = vec4(1.0);
    if (height <= waterLevel) {
        float t = height / waterLevel; 
        terrainColor = mix(waterDark, waterLight, t);
    } 
    else if (height <= sandLevel) {
        float t = (height - waterLevel) / (sandLevel - waterLevel);
        terrainColor = mix(sandDark, sandLight, t);
    } 
    else {
        float t = (height - sandLevel) / (1.0 - sandLevel);
        terrainColor = mix(grassDark, grassLight, t);
    }

    finalColor = terrainColor * fragColor * colDiffuse;
}
