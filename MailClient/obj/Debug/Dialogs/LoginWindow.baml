   M S B A M L   `   `   `  ����E  AMailClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:MailClient.DialogsMailClient.Dialogs  5   	   $    MailClient.Dialogs.LoginWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:MailClient.Dialogs     5   	   -   5          � Title$  Вход��5   	   $	��450��5	   	   $	��800��5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � Topmost . .��5      ]� .+�5   
   �� $��Center=�5      .+�5      �� -   5         ��Name$ 
loginField��5      $��25��5      $	��325��5      $��15q�5      .��5       � 	Resources 5         hinttext j� 
AlignmentX j� Stretch j� TileMode !� Visual��%�   &	        !� 5      $
 Left=�5      $
 None=�5      $
 None=�5       5       �� 5!      $��
Логин��5"   !   ���#959595
�������5$   !   5'      5(      .��6%   ��5)      �� 5*      	 �� 
TargetType"	 ��6   .�5+      �� 
 �� Property
 � �� �� Value$ Center=�6>   �� 5,      
 � ��$ 12(�65   �� 5-      
 � ��$
 Auto��62   �� 5.      
 � ��$
 Auto��62    �� Triggers 5/      P� 50       P� Property � �� P� Value$  ��66   . �51   "   �� 
 � ��# [  6?   52      P� 53       � �� 66   Z� . �54   "   �� 
 � ��# [  6?   55      57      .��58   !   59      G� 5:      -   5         G�Name$ passwordField��5;      $��25��5<      $	��325��5=      $��15q�5>      �� 5@      -   5        $��25��5A      $	��250��5B      $��15q�5C      $��
Войти��5D      5E      5G      5H      5I      