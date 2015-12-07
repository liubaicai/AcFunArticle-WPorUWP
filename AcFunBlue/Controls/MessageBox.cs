using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace AcFunBlue.Controls
{
    public sealed class MessageBox
    {
        // 摘要: 
        //     显示包含指定文本和“确定”按钮的消息框。
        //
        // 参数: 
        //   messageBoxText:
        //     要显示的消息。
        //
        // 异常: 
        //   System.ArgumentNullException:
        //     messageBoxText 为 null。
        public async static Task<MessageBoxResult> Show(string messageBoxText)
        {
            if (messageBoxText == null)
                throw new ArgumentNullException("messageBoxText is null");

            var tcs = new TaskCompletionSource<MessageBoxResult>();

            var dialog = new MessageDialog(messageBoxText);

            dialog.Commands.Add(new UICommand("确定", command =>
            {
                tcs.SetResult((MessageBoxResult)command.Id);
            }, MessageBoxResult.OK));

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();

            return await tcs.Task;
        }

        //
        // 摘要: 
        //     显示包含指定文本、标题栏标题和响应按钮的消息框。
        //
        // 参数: 
        //   messageBoxText:
        //     要显示的消息。
        //
        //   caption:
        //     消息框的标题。
        //
        //   button:
        //     一个值，用于指示要显示哪个按钮或哪些按钮。
        //
        // 返回结果: 
        //     一个值，用于指示用户对消息的响应。
        //
        // 异常: 
        //   System.ArgumentNullException:
        //     messageBoxText 为 null。- 或 -caption 为 null。
        //
        //   System.ArgumentException:
        //     button 不是有效的 System.Windows.MessageBoxButton 值。
        public async static Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            if (messageBoxText == null)
                throw new ArgumentNullException("messageBoxText is null");
            if (caption == null)
                throw new ArgumentNullException("caption is null");
            if (button != MessageBoxButton.OK && button != MessageBoxButton.OKCancel)
                throw new ArgumentException("button is null");

            var tcs = new TaskCompletionSource<MessageBoxResult>();

            var dialog = new MessageDialog(messageBoxText,caption);

            dialog.Commands.Add(new UICommand("确定", command =>
            {
                tcs.SetResult((MessageBoxResult)command.Id);
            }, MessageBoxResult.OK));

            if (button==MessageBoxButton.OKCancel)
            {
                dialog.Commands.Add(new UICommand("取消", command =>
                {
                    tcs.SetResult((MessageBoxResult)command.Id);
                }, MessageBoxResult.Cancel));
            }

            dialog.DefaultCommandIndex = 0;
            if(button==MessageBoxButton.OKCancel)
                dialog.CancelCommandIndex = 1;
            else
                dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();

            return await tcs.Task;
        }
    }

    // 摘要: 
    //     指定显示消息框时要包含的按钮。
    public enum MessageBoxButton
    {
        // 摘要: 
        //     仅显示“确定”按钮。
        OK = 0,
        //
        // 摘要: 
        //     同时显示“确定”和“取消”按钮。
        OKCancel = 1,
    }

    // 摘要: 
    //     表示对消息框的用户响应。
    public enum MessageBoxResult
    {
        // 摘要: 
        //     当前未使用此值。
        None = 0,
        //
        // 摘要: 
        //     用户单击了“确定”按钮。
        OK = 1,
        //
        // 摘要: 
        //     用户单击了“取消”按钮或按下了 ESC。
        Cancel = 2,
        //
        // 摘要: 
        //     当前未使用此值。
        Yes = 6,
        //
        // 摘要: 
        //     当前未使用此值。
        No = 7,
    }
}
