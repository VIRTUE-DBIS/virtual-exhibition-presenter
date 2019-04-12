# Usage
# addpackage [repo url] [package directory name] [Assets dirs to include]
# Example: ./addpackage.sh https://github.com/georgejecook/UnityTimelineEvents.git UnityTimelineEvents 
"TimelineEvents/*"

git clone --depth=1 --no-checkout $1 Packages/$2/repo
git submodule add $1 Packages/$2/repo
git -C Packages/$2/repo/ config core.sparseCheckout true
git submodule absorbgitdirs
echo Assets/$3 >> .git/modules/Packages/$2/repo/info/sparse-checkout
git submodule update --init --force --checkout Packages/$2/repo
