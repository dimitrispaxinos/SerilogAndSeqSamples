   M S B A M L   `   `   `  ����  SerilogSample    SerilogSample.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  SeriLog Sample��5   	   $	��350��6    $	��525��6-   .��5      �� .@�5   
   �� $��Basic Cases��6   .��5      �  � RowDefinitions 5      �� 5	      $
��Auto �6$   �� 5
      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5      �� -   5        $��1��5      $��Create Info��6&   6<   �� 5      -   5        $��2��5      $��Create Error��6&   6=   �� 5      -   5        $��3��5      $,��&Create Error with object and Exception��6&   6W   �� 5      -   5        $��6��5      $*��$Create Entry Enriched With User Info��6&   6U   5      5      �� 5   
   $��Contextual Logging��6   .��5      �  5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5       $
��Auto �6$   �� 5!      $
��Auto �6$   �� 5"      $
��Auto �6$   �� 5#      $
��Auto �6$   �� 5$      $
��Auto �6$   .+�5&      �� -   5        $��1��5&      $!��Add One Property To Context��6&   6L   �� 5'      -   5        $��2��5'      $&�� Add a complete structured Object��6&   6Q   �� 5(      -   5        $��3��5(      $��Add a dynamic Object��6&   6E   �� 5)      -   5        $��4��5)      $'��!Add MultipleProperties To Context��6&   6R   �� 5*      -	   5        $��5��5*      $��Add Class Name As Context��6&   6J   �� 5+      -
   5        $��6��5+      $E��?Add MultipleProperties To Context using Custom PropertyAttacher��6&   6p   �� 5,      -   5        $��7��5,      $!��Add Context using Enrichers��6&   6L   5-      5.      �� 50   
   $��
Exceptions��6   52      �� 54   
   $)��#Timing Cases - Calling Web Services��6   .��55      �  56      �� 57      $
��Auto �6$   �� 58      $
��Auto �6$   �� 59      $
��Auto �6$   �� 5:      $
��Auto �6$   �� 5;      $
��Auto �6$   �� 5<      $
��Auto �6$   �� 5=      $
��Auto �6$   �� 5>      $
��Auto �6$   �� 5?      $
��Auto �6$   .+�5A      �� -   5        $��1��5A      $!��Log Successful Service Call��6&   6L   �� 5B      -   5        $��2��5B      $6��0Produce Warning For Service Call That Takes Long��6&   6a   5C      5D      5E      5H      