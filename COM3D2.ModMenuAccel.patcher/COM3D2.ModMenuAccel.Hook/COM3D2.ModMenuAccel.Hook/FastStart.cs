using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;

namespace COM3D2.ModMenuAccel.Hook
{
    class FastStart
    {
        // the credit for the whole thin goes to はてな (twitter @hatena_37 )
        // I only rewrote method signatures in accordance with  Mono.Cecil.Inject requirements and not to cause excessive overrides

        // ※FileSystemArchiveとFileSystemWindowsでそれぞれ別の手段で高速化を実施しているため注意



        public static bool FileSystemArchiveGetFileListAtExtension(out string[] strList, string extension) //inject at start; pass params; modify return
        {
            strList = null;
            if (extension == ".menu")
            {
                // FileSystemArchiveのGetFileListAtExtensionは全てのファイルを検索対象とするため時間がかかる
                // CM3D2ではmenuは全てmenu配下にあったため、GameUty.m_FileSystem.GetList("menu", AFileSystemBase.ListType.AllFile)でmenu配下だけを検索し取得ができたが、
                // COM3D2ではmodelやtexなどと合わせて全て「parts」配下になったため、同様の処理では取得できなくなり、GameUty.m_FileSystem.GetFileListAtExtension(".menu")となったと思われる
                // しかし、全体を対象とするよりは、「menu」「parts」配下だけを対象としたほうが早いため、下記のように処理を変更
                // 自分の環境では結果のmenu数を比較したところ全て読めているようだった
                // ※自分でarcファイルを再梱包し、「menu」「parts」配下以外の場所にmenuファイルを入れた場合は公式処理では読めるが、この処理では読めなくなる
                string[] menuList = GameUty.FileSystem.GetList("menu", AFileSystemBase.ListType.AllFile);
                string[] partsList = Array.FindAll<string>(GameUty.FileSystem.GetList("parts", AFileSystemBase.ListType.AllFile), (Predicate<string>)(i => new Regex(".*\\.menu$").IsMatch(i)));

                // 2つの配列を合わせた配列を作成

                strList = new string[menuList.Length + partsList.Length];
                Array.Copy(menuList, strList, menuList.Length);
                Array.Copy(partsList, 0, strList, menuList.Length, partsList.Length);
            }
            return strList!=null;
        }



        public static void FileSystemWindowsAddFolderPost(FileSystemWindows fileSystem) //inject at start; pass invoke
        {
            // 公式の処理ではフォルダをひとつひとつAddAutoPathで追加していたが、フォルダが増えるたびにどんどん遅くなっていく
            // AddAutoPathForAllFolderの中身は見えないのでわからないが、使ってみたところ同じ結果を得られそうで、処理速度も大幅に速くなった
            // 現在は「Mod」フォルダのみAddFolderされているのでAddFolder後に処理を実行しているが、
            // もし今後「Mod」フォルダ以外にもAddFolderする場合があれば別の場所でまとめて実行したほうがいいと思われる
            fileSystem.AddAutoPathForAllFolder();
        }

        public static bool FileSystemWindowsAddAutoPath(out bool result) // inject at start; modify return
        {
            // 上記処理でAddAutoPathForAllFolder()を実施しているためこちらは不要
            // 何もせずに処理終了する
            // 関数の処理自体を置き換えるよりは、できれば呼び元の処理を変更して呼ばないようにした方がいいと思われる
            // 自分には難しいため簡単にこのようにした
            result = true;
            return result;
        }


        // これ以降全てFileSystemWindows.GetFileListAtExtensionの置き換え処理
        public static bool FileSystemWindowsGetFileListAtExtension(out string[] result , string extension) // inject at start; pass parameters; modify return
        {
            // FileSystemWindowsのGetFileListAtExtensionはなぜか遅いので、単純に自分で作成した処理でファイルを探す
            // 「Mod」フォルダだけを対象としているため、他の場所も対象とする場合改造が必要
            // 何度も呼ばれる可能性を考慮し、extensionの情報を持つファイル情報リストを作っておき、そこから検索する
            result = GetFilePathListByExtention(extension);
            return true;
        }

        public static readonly string com3d2Path = Directory.GetCurrentDirectory();
        public static readonly string modPath = Path.Combine(com3d2Path, "Mod\\");

        private static List<ModPathInfo> filePathInfoList;

        // 静的コンストラクタ
        static FastStart()
        {
            GameDataFileInfoInit();
        }

        // 初期化処理
        static private void GameDataFileInfoInit()
        {
            filePathInfoList = new List<ModPathInfo>();

            string[] filePathArray = Directory.GetFiles(modPath, "*", SearchOption.AllDirectories);

            foreach (string filePath in filePathArray)
            {
                filePathInfoList.Add(new ModPathInfo(filePath));
            }
        }

        // Mod配下の拡張子がextensionのファイルのパスをすべて返す
        static public string[] GetFilePathListByExtention(string extension)
        {
            // 「.」を削除する
            extension = extension.Replace(".", string.Empty);

            // LINQを使用
            return filePathInfoList
                    .Where(x => x.extention == extension)
                    .Select(x => x.filePath)
                    .ToArray();
        }

        private class ModPathInfo
        {
            public readonly string filePath;
            public readonly string fileName;
            public readonly string extention;

            public ModPathInfo(string filePath)
            {
                // ゲームのパスに合わせるため、filePathには「Mod」フォルダからの相対パスを設定
                Uri u1 = new Uri(modPath);
                Uri u2 = new Uri(filePath);
                Uri relativeUri = u1.MakeRelativeUri(u2);
                string relativePath = relativeUri.ToString();
                relativePath = relativePath.Replace('/', '\\');

                this.filePath = relativePath.ToLower();
                fileName = Path.GetFileName(filePath);
                extention = Path.GetExtension(filePath);
                // ピリオドを削る
                if (extention.Length != 0)
                {
                    extention = extention.Remove(0, 1);
                }
            }
        }
    }
}
