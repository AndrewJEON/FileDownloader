   M S B A M L   `   `   `  ����I  EFileDownloader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:FileDownloaderFileDownloader  5   	       FileDownloader.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:FileDownloader     5   	   -   5          � Title$  FileDownloader��5   	   $	��200��6    $	��600��6-    � WindowStartupLocation$ CenterScreen=�5	       .��5      �  � ColumnDefinitions 5   
   �� 5      $��40 �6   �� 5      $��60 �6   �� 5      �� 5      �� 5      �� 5      $��40 �6    � RowDefinitions 5   
   �� 5      �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��20 �6   �� 5      $��10 �6   �� 5      $��30 �6   �� 5      .+�5   
   �� $��1��6   $��1��6!   $
��URL:��61   $��Center=�5      $��Center=�61   $��16(�5       �� 5!   
   -   5         ��Name$ 
UrlTextBox��5!   C   $��1��6   $��2��6   $��3��6/   $��Center=�5"      $��25��6-   � 5#   
   -   5         �Name$ Progress��5$      $��2��5#      $��2��6#   $��3��63   $��25��5$   (    � Maximum$
 1000Q�64   $y�	Collapsed=�5%       � Value 5&      �� *ProgressVal+�� 5'   
   -   5         ��Name$ RemainTimeTxt��5(      $��3��5'      $��2��6!   ��5(   +   �� *
RemainTime+�� 5)   
   -   5        $ SpeedTxt��5*      $��3��5)      $��3��6!   $��Center=�5*   @   ��6&   �� *
CurSpeed+6@   �� 5+   
   -   5        $ DispalySizeTxt��5,      $��3��5+      $��4��6!   $��Right=�61   ��5,   ,   �� *DisplaySize+�� 5.   
   	 �� Orientation$	 
Horizontal=�6   $��Center=�6.   $��5��6K   $��0��6X   $��6��6h   .+�5/      �� -   5        
 ��Name$
 DownloadBtn��5/      $��下载��6*   $	��Top=�67   $	��100��50      $��30��6   6*   �� 51      $��Center=�6   �� 52      53      55      56      