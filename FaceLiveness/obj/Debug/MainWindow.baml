   M S B A M L   `   `   `  ����G  CFaceLiveness, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:FaceLivenessFaceLiveness  5   	       FaceLiveness.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:FaceLiveness     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)    � WindowState$ 	Maximized=�5	   	   .��5      � .+�5   
   � -   5         �Name$ loadingPopup��5      ��.   � 	StaysOpen .  $��Center=�6G   .��5      �� 
�������
���   �$��2q�6<    �� CornerRadius$ 10��6P    �� Padding$ 10q�6b   ._�5      �� $	��450��6   $��Center=�6)   $��Center=�6F   .+�5      �� $.��(Checking Image quality and face liveness��6!   $
��Wrap=�5   !   $��20(�5   !   
���   � �� TextAlignment$ Center=�5   !   $��	0,10,0,10q�5   !   � 5      $	��450��6"   $��20��6.    � IsIndeterminate . 6:   5      5      5      � 5   
   -   5        $ instructionPopup��5      ��.   .  $��Center=�6K   .��5      �� 
�������
���   �$��2q�6<   $ 10��6P   $ 10q�6b   ._�5      �� $	��450��6   $��Center=�6)   $��Center=�6F   .+�5      �� -   5         ��Name$ livenessLabel��5%       $��Liveness��5       $
��Wrap=�5        $��24(�5!       
���   �$
 Left=�5#       $��0,0,0,10q�5$       5%       �� 5'      $D��>The captured image does not meet NADRA's quality requirements.��6    $
��Wrap=�5(       $��24(�5)       
���   �$
 Left=�5+       $��0,0,0,10q�5,       �� 5-      $��Please ensure:��6    $
��Wrap=�5.       $��18(�5/       
���   �$
 Left=�51       �� 53      $<��6* There is no overexposure or glare in the background.��6    $
��Wrap=�54       $��18(�55       
���   �$
 Left=�57       �� 59      $S��M* The face is clearly visible and not covered (e.g., no sunglasses or masks).��6    $
��Wrap=�5:       $��18(�5;       
���   �$
 Left=�5=       �� 5?      $=��7* The eyes are open and looking directly at the camera.��6    $
��Wrap=�5@       $��18(�5A       
���   �$
 Left=�5C       $��0,0,0,10q�5D       �� 5F      $9��3Kindly recapture the image to meet these standards.��6    $
��Wrap=�5G       $��20(�5H       
���   �$
 Left=�5J       �� 5M      -   5        $��OK��5M      $	��100��6*   $��0,10,0,0q�66   $��Center=�6H   5N      5O      5P      5Q      �� 5S   
   -   5        	 ��Name$	 videoPlayer��5S      $��
10,0,10,70q�6%   �� 5T   
   
 �� Orientation$
 Vertical=�6   $��Bottom=�6,   .+�5W      �� -   5         ��Name$ messageLabel��5W      $��Center=�5X      $��Bottom=�5Y      $-��'Please place your face inside the ovals��5Z      $��24(�5[      
���  ��5\      �� 5^      $
 
Horizontal=�6   $��0,0,0,10q�62   $��Center=�6D   .+�5`      �� $��24��5a      $��5,0,0,0q�5b      $
��Left=�5c      $��Bottom=�5d      $��Select video source��5e      �� 5g      -   5         ��Name$ comboBox��5g      $��5,0,0,0q�5h      $	��167��5i      $��21��5j      $
��Left=�5k      $��Bottom=�5l       �� DisplayMemberPath$
 Name��5m      ��5n      �� *VideoDevices+ �� SelectedItem 5o      �� *CurrentDevice+�� 5r      -   5        $��67��5r      $��5,0,0,0q�5s      $��21��5t      $
��Left=�5u      $��Bottom=�5v      $��Start��5x      5�      5�      5�      5�      