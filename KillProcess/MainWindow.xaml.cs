using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace KillProcess;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        txtKey.Focus();
    }

    private void BtnSearch_Click(object sender, RoutedEventArgs e)
    {
        SearchProcesses();
    }

    private void SearchProcesses()
    {
        lstProcessList.Items.Clear();
        
        var keyword = txtKey.Text.Trim().ToLower();
        var processes = Process.GetProcesses();
        var matchedCount = 0;

        foreach (var process in processes)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword) || process.ProcessName.ToLower().Contains(keyword))
                {
                    lstProcessList.Items.Add(process.ProcessName);
                    matchedCount++;
                }
            }
            catch
            {
                // 忽略无法访问的进程
            }
        }

        if (lstProcessList.Items.Count > 0)
        {
            lstProcessList.SelectedIndex = 0;
        }

        UpdateStatus(matchedCount);
    }

    private void UpdateStatus(int count)
    {
        txtStatus.Text = count > 0 
            ? $"找到 {count} 个进程" 
            : "未找到匹配的进程";
    }

    private void BtnKillProcess_Click(object sender, RoutedEventArgs e)
    {
        if (lstProcessList.SelectedItem == null)
        {
            MessageBox.Show("请先选择一个进程", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var processName = lstProcessList.SelectedItem.ToString();
        
        var result = MessageBox.Show(
            $"确定要终止进程 \"{processName}\" 吗？\n\n这可能会导致数据丢失。",
            "确认终止",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            var processes = Process.GetProcessesByName(processName!);
            var killedCount = 0;

            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    killedCount++;
                }
                catch
                {
                    // 某些进程可能无法终止
                }
            }

            if (killedCount > 0)
            {
                MessageBox.Show(
                    $"成功终止 {killedCount} 个 \"{processName}\" 进程",
                    "成功",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    "无法终止该进程，可能需要管理员权限",
                    "失败",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            // 刷新列表
            SearchProcesses();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"终止进程时出错：{ex.Message}",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void TxtKey_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SearchProcesses();
            e.Handled = true;
        }
    }

    private void LstProcessList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        btnKillProcess.IsEnabled = lstProcessList.SelectedItem != null;
    }
}

