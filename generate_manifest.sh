#!/bin/sh

# Extract information from .csproj file
name=$(flatpak-spawn --host xmlstarlet sel -t -v "//PropertyGroup/Product" "$CURRENT_PROJECT"/"$CURRENT_PROJECT".csproj)
version=$(flatpak-spawn --host xmlstarlet sel -t -v "//PropertyGroup/Version" "$CURRENT_PROJECT"/"$CURRENT_PROJECT".csproj)
description=$(flatpak-spawn --host xmlstarlet sel -t -v "//PropertyGroup/Description" "$CURRENT_PROJECT"/"$CURRENT_PROJECT".csproj)
website=$(flatpak-spawn --host xmlstarlet sel -t -v "//PropertyGroup/Source" "$CURRENT_PROJECT"/"$CURRENT_PROJECT".csproj)

# Generate JSON content
manifest=$(cat <<EOF
{
    "name": "$name",
    "version_number": "$version",
    "website_url": "$website",
    "description": "$description",
    "dependencies": [
        "BepInEx-BepInExPack-5.4.2100",
        "Rune580-LethalCompany_InputUtils-0.7.1",
        "TestAccount666-TestAccountCore-1.1.0"
    ]
}
EOF
)

# Write JSON content to manifest file
echo "$manifest" > BuildOutput/manifest.json
