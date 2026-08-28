# Custom Attributes & Reflection - Permission System

مشروع تعليمي بسيط باستخدام **C#** يشرح كيفية استخدام **Custom Attributes** مع **Reflection** لبناء نظام بسيط للتحقق من **Permissions**.

---

## فكرة المشروع

الفكرة هي تحديد الـ **Permission** المطلوبة لتنفيذ كل `Method` باستخدام `Custom Attribute`.

مثال:

```csharp
[RequirePermission("Users.Delete")]
public void DeleteUser()
{
    Console.WriteLine("User Deleted");
}
```

السطر:

```csharp
[RequirePermission("Users.Delete")]
```

يعني أن الـ `Method` تحتاج إلى الـ Permission التالية:

```text
Users.Delete
```

لكن الـ `Attribute` نفسها لا تقوم بمنع المستخدم.

هي فقط تقوم بتخزين هذه المعلومة كـ **Metadata**.

بعد ذلك نستخدم **Reflection** لقراءة هذه الـ Metadata والتحقق من صلاحيات المستخدم.

---

# كيف تعمل الفكرة؟

```text
Custom Attribute
       ↓
تخزين Permission كـ Metadata
       ↓
Reflection
       ↓
قراءة الـ Attribute
       ↓
معرفة الـ Permission المطلوبة
       ↓
فحص Permissions الخاصة بالمستخدم
       ↓
Access Granted / Access Denied
```

---

# 1. إنشاء Custom Attribute

نبدأ بإنشاء `Class` ترث من `Attribute`:

```csharp
public class RequirePermissionAttribute : Attribute
{
}
```

السطر:

```csharp
: Attribute
```

يعني أن `RequirePermissionAttribute` هي Custom Attribute وليست `Class` عادية.

---

# 2. تحديد أين يمكن استخدام الـ Attribute

نستخدم:

```csharp
[AttributeUsage(AttributeTargets.Method)]
```

`AttributeUsage` هي نفسها Attribute موجودة في .NET.

أما:

```csharp
AttributeTargets.Method
```

فتعني أن `RequirePermissionAttribute` يمكن استخدامها فقط على الـ `Methods`.

مثال صحيح:

```csharp
[RequirePermission("Users.Delete")]
public void DeleteUser()
{
}
```

لكن لا يمكن استخدامها على `Class` أو `Property` لأننا حددنا:

```csharp
AttributeTargets.Method
```

---

# 3. تخزين الـ Permission

داخل الـ Attribute لدينا:

```csharp
public string Permission { get; }
```

هذه الـ Property تخزن اسم الـ Permission المطلوبة.

ثم لدينا Constructor:

```csharp
public RequirePermissionAttribute(string permission)
{
    Permission = permission;
}
```

عندما نكتب:

```csharp
[RequirePermission("Users.Delete")]
```

يتم تمرير:

```text
"Users.Delete"
```

إلى الـ Constructor.

فتصبح قيمة:

```csharp
Permission
```

هي:

```text
Users.Delete
```

---

# 4. استخدام الـ Attribute على Methods

لدينا `UserService` يحتوي على عدة عمليات:

```csharp
[RequirePermission("Users.Create")]
public void CreateUser()
{
    Console.WriteLine("User Created");
}
```

هذه الـ Method تحتاج:

```text
Users.Create
```

ولدينا:

```csharp
[RequirePermission("Users.Delete")]
public void DeleteUser()
{
    Console.WriteLine("User Deleted");
}
```

وهذه تحتاج:

```text
Users.Delete
```

ولدينا:

```csharp
[RequirePermission("Users.View")]
public void ViewUsers()
{
    Console.WriteLine("Displaying Users");
}
```

وهذه تحتاج:

```text
Users.View
```

---

# 5. إنشاء User

نحتاج إلى معرفة Permissions التي يمتلكها المستخدم.

لذلك لدينا:

```csharp
public class User
{
    public string Name { get; set; }

    public List<string> Permissions { get; set; }
}
```

`Name` يخزن اسم المستخدم.

أما:

```csharp
List<string> Permissions
```

فتخزن جميع الـ Permissions التي يمتلكها المستخدم.

مثال:

```csharp
User user = new User
{
    Name = "Yaseen",

    Permissions = new List<string>
    {
        "Users.Create",
        "Users.View"
    }
};
```

هذا المستخدم يمتلك:

```text
Users.Create
Users.View
```

لكنه لا يمتلك:

```text
Users.Delete
```

---

# 6. استخدام Reflection

الآن نريد فحص `DeleteUser`.

أولًا نحصل على `Type` الخاصة بـ `UserService`:

```csharp
Type type = typeof(UserService);
```

الـ `Type` تعطينا معلومات عن الـ `Class`.

ومن خلالها يمكننا استخدام Reflection للوصول إلى:

```text
Methods
Properties
Fields
Attributes
```

---

# 7. الحصول على Method

نستخدم:

```csharp
MethodInfo method = type.GetMethod("DeleteUser");
```

هنا نقول للـ Reflection:

> ابحث لي عن الـ Method التي اسمها `DeleteUser`.

النتيجة تكون كائنًا من النوع:

```csharp
MethodInfo
```

ويمثل الـ `DeleteUser` Method.

---

# 8. قراءة الـ Custom Attribute

الآن لدينا `MethodInfo`.

نريد معرفة هل توجد عليها `RequirePermissionAttribute`.

نستخدم:

```csharp
RequirePermissionAttribute attribute =
    method?.GetCustomAttribute<RequirePermissionAttribute>();
```

هذا السطر هو أهم سطر في المثال.

نحن نقول:

> اذهب إلى هذه الـ Method واقرأ الـ Custom Attribute من نوع `RequirePermissionAttribute`.

إذا كانت موجودة، نحصل على object يمثل الـ Attribute.

---

# 9. معرفة الـ Permission المطلوبة

بعد أن نحصل على:

```csharp
attribute
```

يمكننا الوصول إلى:

```csharp
attribute.Permission
```

وبالنسبة إلى `DeleteUser` ستكون القيمة:

```text
Users.Delete
```

إذًا Reflection سمحت لنا بالانتقال من:

```text
DeleteUser
```

إلى:

```text
Users.Delete
```

بدون أن نكتب Permission بشكل منفصل داخل منطق التحقق.

---

# 10. التحقق من وجود Attribute

نكتب:

```csharp
if (attribute == null)
```

إذا كانت `attribute` تساوي `null` فهذا يعني أن الـ Method لا تحتوي على:

```csharp
[RequirePermission(...)]
```

وفي هذه الحالة يمكننا تنفيذ الـ Method مباشرة:

```csharp
method?.Invoke(service, null);
```

---

# 11. فحص Permission الخاصة بالمستخدم

إذا كانت الـ Method تحتوي على `RequirePermissionAttribute`، نتحقق من صلاحية المستخدم:

```csharp
else if (user.Permissions.Contains(attribute.Permission))
```

هنا:

```csharp
attribute.Permission
```

تحتوي على:

```text
Users.Delete
```

و:

```csharp
user.Permissions
```

تحتوي على:

```text
Users.Create
Users.View
```

لذلك:

```csharp
Contains(...)
```

سترجع:

```text
false
```

---

# 12. Access Denied

إذا لم يمتلك المستخدم الـ Permission المطلوبة:

```csharp
Console.WriteLine("Access Denied");
```

ولا يتم تنفيذ الـ Method.

في مثالنا:

```text
Required Permission:
Users.Delete
```

لكن المستخدم يمتلك:

```text
Users.Create
Users.View
```

لذلك النتيجة:

```text
Access Denied
```

---

# 13. Access Granted

لو أضفنا:

```text
Users.Delete
```

إلى Permissions الخاصة بالمستخدم:

```csharp
Permissions = new List<string>
{
    "Users.Create",
    "Users.View",
    "Users.Delete"
};
```

التحقق:

```csharp
user.Permissions.Contains(attribute.Permission)
```

سيعيد:

```text
true
```

ثم:

```csharp
method.Invoke(service, null);
```

سيقوم بتنفيذ `DeleteUser`.

والنتيجة:

```text
Access Granted
User Deleted
```

---

# 14. ما هو Invoke()؟

لدينا:

```csharp
method.Invoke(service, null);
```

`method` هو `MethodInfo`.

و:

```csharp
Invoke()
```

تسمح لنا بتنفيذ الـ Method الموجودة داخل `MethodInfo`.

في المثال:

```text
method → DeleteUser()
```

لذلك:

```csharp
method.Invoke(service, null);
```

تعني تنفيذ:

```csharp
service.DeleteUser();
```

لكن بطريقة **Dynamic** باستخدام Reflection.

---

# 15. أهم نقطة في المشروع

هذا السطر:

```csharp
[RequirePermission("Users.Delete")]
```

لا يقوم وحده بالتحقق من الـ Permission.

الـ Attribute فقط تخزن:

```text
Users.Delete
```

أما عملية التحقق الفعلية فتتم بواسطة الكود الذي كتبناه باستخدام:

```text
Reflection
+
Permission Checking Logic
```

---

# الصورة الكاملة

```text
[RequirePermission("Users.Delete")]
                ↓
        Custom Attribute
                ↓
       Stores Metadata
                ↓
            Reflection
                ↓
    GetCustomAttribute<T>()
                ↓
       attribute.Permission
                ↓
         "Users.Delete"
                ↓
     Check User Permissions
                ↓
       ┌────────┴────────┐
       ↓                 ↓
    Granted            Denied
       ↓                 ↓
    Invoke()        Access Denied
```

---

# العلاقة بين المفاهيم

### Custom Attribute

تستخدم لإضافة **Metadata** إلى الكود.

```csharp
[RequirePermission("Users.Delete")]
```

### Reflection

تستخدم لقراءة الـ Metadata في وقت التشغيل:

```csharp
method.GetCustomAttribute<RequirePermissionAttribute>();
```

### Permission Checking

يتم استخدام الـ Metadata لمعرفة ماذا يحتاج المستخدم:

```csharp
user.Permissions.Contains(attribute.Permission)
```

### Invoke

إذا كان المستخدم يمتلك الصلاحية، يتم تنفيذ الـ Method:

```csharp
method.Invoke(service, null);
```

---

# لماذا هذا المثال مهم؟

هذا المثال يجمع عدة مفاهيم مهمة في C# في مشروع صغير:

```text
C#
│
├── OOP
├── Attributes
├── Custom Attributes
├── Metadata
├── Reflection
├── MethodInfo
├── Generics
├── Collections
└── Authorization Concept
```

والفكرة الأساسية التي يجب فهمها هي:

```text
Attribute = تخزين Metadata
Reflection = قراءة Metadata
Authorization Logic = اتخاذ القرار
```

---

## ملاحظة

هذا المشروع **Educational Example** لفهم العلاقة بين `Custom Attributes` و`Reflection` و`Authorization`.

هو ليس نظام **Production-Ready Authorization**، ولا يُقصد استخدامه كما هو في تطبيق حقيقي.

في تطبيقات **ASP.NET Core** الحقيقية توجد أنظمة `Authentication` و`Authorization` و`Policies` و`Handlers` جاهزة وأكثر أمانًا ومرونة.

---

## Technologies

* C#
* .NET
* Custom Attributes
* Reflection
* MethodInfo
* Permissions
* Authorization Concepts
