   M S B A M L   `   `   `  ����O  KWindowsProgrammering, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WindowsProgrammering.ViewWindowsProgrammering.View  5      *    $WindowsProgrammering.View.ExpanderUC   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:WindowsProgrammering.View     5      $
��auto��5   3   
  � Tag  5      �� *DataContext+ �� RelativeSource ��  �� AncestorType" �.��5	      � $��
0,0,0,-445q�6   .+�5
      9� -   5         9�Name$ expander��5
       9� ExpandDirection$
 Left=�6!   $��
0,10,0,375q�68   .��5   
   �� .+�5      �� $��Class��6   .��5      d� -   5         d�Name$ classDiagramButton��5   N   $��	Add class��6   $
��Auto��63   $	��150��6A   $��9,0,6,3q�5      ��60   �� *AddClassCommand+5      �� 5      $��Arrow��6   .��5      d� -   5        $ ArrowButton��5   S   $��Association��6#   $
��Auto��69   $	��150��6G   $��9,0,6,2q�5       J� RenderTransformOrigin$ 0.501,1.684&�6_   ��60   �� *AddAssociationArrowCommand+6_   5      �� 5      .��5      d� -   5        $ 
ArroButton��5   J   $��Normal��6   $
��Auto��60   $	��150��6>   $��9,0,6,4q�5      ��60   �� *AddNormalArrowCommand+5      5      5      5      5      