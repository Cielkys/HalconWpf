# HalconWpf 项目长期记忆

## 环境
- HALCON 17.12-Progress，安装于 `C:\Program Files\MVTec\HALCON-17.12-Progress`（`%HALCONROOT%`，`%HALCONARCH%=x64-win64`）。
- `halcondotnet.dll` 位于 `%HALCONROOT%\bin\dotnet35\`（纯 IL），原生库在 `%HALCONROOT%\bin\x64-win64\`，PATH 已含该目录。
- 实测 halcondotnet 17.12 在 net8.0-windows 与 net48 下均可加载、读图、创建 WPF 控件；许可证可用（无 license_*.dat 但能 checkout）。
- XAML 中使用 `HalconDotNet.HSmartWindowControlWPF` 必须额外引用 NuGet 包 `System.Drawing.Common`（否则 MC1000 找不到 System.Drawing.Common）。

## 界面结构（2026-09-22 搭建）
- `Views/MainWindow.xaml(.cs)`：顶部工具栏（打开图片/清空输出）+ 底部状态栏（状态、图片信息、鼠标 row/col）+ 中间左 `OutputParamsView`、右 `HalconView`（GridSplitter 分隔）。命名空间仍为 `HalconWpf.MainWindow`。
- `Views/HalconView.xaml(.cs)`：UserControl = 自带功能栏（打开/适应窗口）+ `HSmartWindowControlWPF`（HMoveContent、WheelForwardZoomsIn、HKeepAspectRatio、HDoubleClickToFitContent）。代码后台仅：Loaded 交窗口、鼠标 row/col 转发。
- **用户偏好（2026-09-22）**：工具栏不用 `ToolBarTray`/`ToolBar`，改用 `Border + 横向 StackPanel + Button`；顶部栏不放"关于"按钮；功能栏不放"放大/缩小"按钮（滚轮缩放保留）。
- `Views/OutputParamsView.xaml(.cs)`：UserControl，DataGrid 显示输出参数（参数/值 两列，**无索引列**，09-22 应用户要求删除），`SetParameters(IEnumerable<OutputParameter>)`、`Clear()`。
- `Models/OutputParameter.cs`：输出参数模型（Name + double[] Values），用于承载 area[]、row[]、col[] 等数组输出。
- `App.xaml` StartupUri = `/HalconWpf;component/Views/MainWindow.xaml`（修正过：原 "MainWindow.xaml" 找不到 Views 下的窗口）。
- csproj：net8.0-windows + WPF，引用 `$(HALCONROOT)\bin\dotnet35\halcondotnet.dll`。

## 编码注意
- **MVVM 框架（2026-09-22 定稿）**：一律用 `CommunityToolkit.Mvvm`（csproj 已引用 8.4.2）——ViewModel 继承框架 `ObservableObject` 并声明为 `partial class`，命令用 `[RelayCommand]` 源生成，**可通知属性一律用 `[ObservableProperty]` 源生成（写在字段上，类须 partial），不要手写 get + SetProperty 属性包装**（2026-09-22 用户指出"有框架怎么还在这样写"，ImageInfo/MousePositionText/StatusText 已改）；不要手写 ObservableObject/RelayCommand（自建的 `Infrastructure/` 已删除）。用户明确要求用现成框架而非自造轮子。
- **架构定稿（2026-09-23 二次修正）**：**3 VM + Halcon 服务接口层、渲染零事件**——`MainViewModel`（组合根：StatusText、OpenImageCommand 委托、组合两个子 VM）+ `HalconViewModel`（图片业务：ImageInfo/MousePositionText/OpenImage/FitWindow 命令，ctor 注入 `IFileDialogService`、`IHalconService`、状态回调、加载完成回调）+ `OutputParamsViewModel`。**Halcon 操作（加载、阈值分割、连通域等）全部放 `Halcon/` 文件夹，VM 只通过 `IHalconService` 接口调用，接口加方法时实现放 `HalconService`。** 视图只做 Loaded 交窗口 + 鼠标转发，服务自己渲染（ImageLoaded/FitWindowRequested/StatusMessage 渲染事件已删，渲染不走事件）。模型 `OutputParameter`/`ParameterRow`（无 Index，数组值逗号拼接）。XAML 嵌套绑定 `Halcon.ImageInfo`，HalconView 子 `DataContext="{Binding Halcon}"`。**⚠️ 严禁主动合并这些 VM（用户两次强调过）。**
- HTuple 有到 int/bool 的隐式转换，`Console.WriteLine(htuple)` 或字符串拼接易引发重载二义性，显式用 `.I/.D/.S`。
- `HSmartWindowControlWPF` 关键 API：`HalconWindow`(HWindow)、`SetFullImagePart(HImage)`、`HImagePart`(Rect)、`HDisplayCurrentObject`、`HMouseMove` 事件参数含 Row/Column/X/Y/Delta/Button。

## 教学偏好（2026-09-22，重要）
- 用户明确批评"直接给成品代码学不到东西"：**教学场景下不要贴完整代码**。正确方式 = 分课讲解原理 → 出练习题（给验收标准、允许查的 API、不许抄的部分）→ 用户自己写并贴回 → 逐行批改 → 进下一课。
- 可以给：概念讲解、思路、API 线索、常见坑、自测问题；不给：可直接抄的完整实现。
- Fsm 项目学习路线（7 课）：1) 串口收发+hex打印 2) 帧格式+模拟器发帧 3) 接收解析 4) 手写 CRC-16/MODBUS 5) 篡改字节实验 6) FSM(超时/防死循环) 7) Modbus 主从触发。
