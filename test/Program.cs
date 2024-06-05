using System;

public class Animal {
    public virtual void MakeSound() {
        Console.WriteLine("Cri cri!");
    }
}

public class Dog : Animal {
    public override void MakeSound() {
        Console.WriteLine("Au au!");
    }
}

public class Program {
    public static void Main() {
        Animal a = new Dog();
        a.MakeSound(); 
    }
}
