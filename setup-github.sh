#!/bin/bash
# ============================================
# LayerTabs - GitHub Setup (Mac/Linux)
# これを実行するだけでGitHubにプッシュ完了
# ============================================

echo ""
echo "╔═══════════════════════════════════════╗"
echo "║   LayerTabs GitHub Setup              ║"
echo "╚═══════════════════════════════════════╝"
echo ""

# ---------(1) Check git
if ! command -v git &> /dev/null; then
    echo "[ERROR] Git がインストールされていません"
    echo "brew install git または Xcode Command Line Tools をインストールしてください"
    exit 1
fi

# ---------(2) Get GitHub username
read -p "GitHubユーザー名を入力: " GITHUB_USER
if [ -z "$GITHUB_USER" ]; then
    echo "[ERROR] ユーザー名が入力されていません"
    exit 1
fi

# ---------(3) Confirm
echo ""
echo "リポジトリURL: https://github.com/$GITHUB_USER/LayerTabs"
echo ""
echo "[重要] 先にGitHubで空のリポジトリを作成してください:"
echo "       https://github.com/new"
echo "       - Repository name: LayerTabs"
echo "       - READMEは追加しない（空のまま）"
echo ""
read -p "準備できたら Enter を押してください..."

# ---------(4) Initialize git
echo ""
echo "[1/4] Git 初期化中..."
if [ -d .git ]; then
    echo "     既に初期化済み - スキップ"
else
    git init
fi

# ---------(5) Add files
echo "[2/4] ファイル追加中..."
git add .

# ---------(6) Commit
echo "[3/4] コミット作成中..."
git commit -m "Initial commit - LayerTabs v1.0.0"

# ---------(7) Push
echo "[4/4] GitHub にプッシュ中..."
git remote remove origin 2>/dev/null
git remote add origin "https://github.com/$GITHUB_USER/LayerTabs.git"
git branch -M main
git push -u origin main

if [ $? -ne 0 ]; then
    echo ""
    echo "[ERROR] プッシュに失敗しました"
    echo "GitHubの認証を確認してください"
    exit 1
fi

echo ""
echo "╔═══════════════════════════════════════╗"
echo "║   完了！                              ║"
echo "╚═══════════════════════════════════════╝"
echo ""
echo "リポジトリ: https://github.com/$GITHUB_USER/LayerTabs"
echo "Actions:    https://github.com/$GITHUB_USER/LayerTabs/actions"
echo ""
echo "GitHub Actions が自動でビルドを開始します（約2-3分）"
echo "完了後、Actions → Artifacts からダウンロードできます"
echo ""
