#!/bin/bash
# ============================================
# LayerTabs - Yak Package Builder (Mac/Linux)
# ============================================

set -e

echo ""
echo "=== LayerTabs Yak Package Builder ==="
echo ""

# Check for Yak
if ! command -v yak &> /dev/null; then
    echo "[ERROR] Yak not found. Install from:"
    echo "        https://developer.rhino3d.com/guides/yak/"
    echo ""
    echo "Or run in Rhino: _PackageManager then search 'yak'"
    exit 1
fi

# Variables
VERSION="1.0.0"
OUTPUT_DIR="dist"

# Create output directory
mkdir -p "$OUTPUT_DIR"

# --------- Build Rhino 8 (Mac) ---------
echo ""
echo "[1/2] Building for Rhino 8..."
dotnet build src/LayerTabs.csproj -c Release -f net7.0
if [ $? -ne 0 ]; then
    echo "[ERROR] Build failed"
    exit 1
fi

# Create package folder
PKG8="$OUTPUT_DIR/rh8"
rm -rf "$PKG8"
mkdir -p "$PKG8"

cp bin/Release/8/LayerTabs.dll "$PKG8/"
cp bin/Release/8/LayerTabs.rhp "$PKG8/" 2>/dev/null || true
cp bin/Release/8/Newtonsoft.Json.dll "$PKG8/"
cp manifest.yml "$PKG8/"
cp resources/icon.png "$PKG8/" 2>/dev/null || true

# Build yak package
echo "[2/2] Creating Rhino 8 yak package..."
cd "$PKG8"
yak build --platform mac
mv *.yak "../layertabs-${VERSION}-rh8-mac.yak"
cd ../..

# Cleanup
rm -rf "$OUTPUT_DIR/rh8"

echo ""
echo "=== Build Complete! ==="
echo ""
echo "Output files in $OUTPUT_DIR/:"
ls -la "$OUTPUT_DIR"/*.yak
echo ""
echo "To install: Double-click the .yak file"
echo "To publish: yak push dist/layertabs-${VERSION}-rh8-mac.yak"
echo ""
