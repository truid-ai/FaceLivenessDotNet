   M S B A M L   `   `   `  ���� G  CFaceLiveness, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:FaceLivenessFaceLiveness      FaceLiveness.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:FaceLiveness       � Title$  
MainWindow��$	��450��$	��800�� � WindowState$ 	Maximized=�.��� .+�� -    �Name$ loadingPopup����.   � 	StaysOpen .  $��Center=�.���� 
�������
���   �$��2q� �� CornerRadius$ 10�� �� Padding$ 10q�._��� $	��450��$��Center=�$��Center=�.+��� $.��(Checking Image quality and face liveness��$
��Wrap=�$��20(�
���   � �� TextAlignment$ Center=�$��	0,10,0,10q�� $	��450��$��20�� � IsIndeterminate . � -   $ instructionPopup����.   .  $��Center=�.���� 
�������
���   �$��2q�$ 10��$ 10q�._��� $	��450��$��Center=�$��Center=�.+��� -    ��Name$ livenessLabel��$��Liveness��$
��Wrap=�$��24(�
���   �$
 Left=�$��0,0,0,10q��� $D��>The captured image does not meet NADRA's quality requirements.��$
��Wrap=�$��24(�
���   �$
 Left=�$��0,0,0,10q��� $��Please ensure:��$
��Wrap=�$��18(�
���   �$
 Left=��� $<��6* There is no overexposure or glare in the background.��$
��Wrap=�$��18(�
���   �$
 Left=��� $S��M* The face is clearly visible and not covered (e.g., no sunglasses or masks).��$
��Wrap=�$��18(�
���   �$
 Left=��� $=��7* The eyes are open and looking directly at the camera.��$
��Wrap=�$��18(�
���   �$
 Left=�$��0,0,0,10q��� $9��3Kindly recapture the image to meet these standards.��$
��Wrap=�$��20(�
���   �$
 Left=��� -   $��OK��$	��100��$��0,10,0,0q�$��Center=��� -   	 ��Name$	 videoPlayer��$��
10,0,10,70q��� 
 �� Orientation$
 Vertical=�$��Bottom=�.+��� -    ��Name$ messageLabel��$��Center=�$��Bottom=�$-��'Please place your face inside the ovals��$��24(�
���  ���� $
 
Horizontal=�$��0,0,0,10q�$��Center=�.+��� $��24��$��5,0,0,0q�$
��Left=�$��Bottom=�$��Select video source���� -    ��Name$ comboBox��$��5,0,0,0q�$	��167��$��21��$
��Left=�$��Bottom=� �� DisplayMemberPath$
 Name������ *VideoDevices+ �� SelectedItem �� *CurrentDevice+�� -   $��67��$��5,0,0,0q�$��21��$
��Left=�$��Bottom=�$��Start��