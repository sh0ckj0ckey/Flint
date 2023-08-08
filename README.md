## <img src="Flint3/Assets/Logos/flint_logo.png" width="24x"> <b>燧石 — 随地查单词</b>

> 经常在电脑上查阅英文内容的朋友，你是否和我有一样的困扰：每当遇到一个陌生的单词，就要打开浏览器 - 在搜索引擎中输入单词 - 然后跳转到结果页面 - 最后从众多结果中找到想要的答案。这繁琐的搜索过程让人身心俱疲，更别提这期间出现的转圈圈和大量广告了。现在，借助这款词典应用"燧石"，整个流程就简单了很多，你只需要打开"燧石"，然后输入单词，甚至都不需要再按回车键，答案就跃然屏幕之上。而且"燧石"使用的是源于 [ECDICT](https://github.com/skywind3000/ECDICT) 的离线词典数据库，因此也无需与网络打交道（虽然这让APP的存储占用大了不少，但这一切都是值得的）。

"燧石"随地查单词，任何问题或者功能需求欢迎联系我😀

点击下方按钮即可下载

<a href="https://apps.microsoft.com/store/detail/9P8735FCS5S9?launch=true&mode=mini">
	<img src="https://get.microsoft.com/images/zh-CN%20dark.svg"/>
</a>

#### TODO
- ~~单例运行~~
- 生词本功能
- 设置中添加一个设置项，控制点击标题栏关闭按钮时是"退出应用"还是"隐藏到托盘"
- SegmentedControl 中添加"历史记录"栏
- 添加联网查词，暂定使用必应词典 API，从而支持中译英功能（设置中切换 联网查词/离线词典，联网模式下搜索栏 Placeholder 改为"按下回车查询"）
- 更新一下截图

#### 一睹芳容
![screenshot.png](README/screenshot.png)

#### 关于 WinUI 3 打包
WinUI 3 目前不像 UWP 支持直接打出 appxbundle、msixbundle 这种类型的包，需要自己手动打出 x64 和 ARM64 的 msix 包，然后这样操作：
将这两个 msix 文件放到例如 C 盘根目录的 Source 文件夹内(不能有其他文件)，然后管理员运行终端如下命令：

```
 "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x86\MakeAppx.exe" bundle /d "C:\Source" /p C:\out.msixbundle
 ```

这样就可以得到一个 msixbundle 包用来上传微软商店了

#### 首页顶部栏控件 SegmentedControl
SegmentedControl 控件从这里下载的nuget包 https://pkgs.dev.azure.com/ms/DevHome/_packaging/DevHomeDependencies/nuget/v3/index.json，后续应当会上架到 nuget.org

© 2023 sh0ckj0ckey.
