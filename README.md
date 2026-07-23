# FrozenBox Tags System

Легковесная, оптимизированная и строго типизированная система тегов для Unity. 
Пакет позволяет уйти от сравнения строк в пользу работы со структурами `TagHandle` и битовыми масками `FlagsHandle`. Особенностью системы является глубокая интеграция с C# Enum и автоматическая конвертация значений.

## 🌟 Ключевые особенности

* **Оптимизация:** Теги хранятся в виде `TagHandle` (ссылка на источник + индекс). Наборы тегов представлены как `FlagsHandle` (маска `int`), что ограничивает один источник 32 тегами для максимальной скорости битовых операций.
* **Component-Based:** Наличие `TagsComponent` со встроенным `TagsStore` для гибкого назначения тегов на объекты.
* **Продвинутая работа с Enum:** Автоматическая генерация пула тегов из C# перечислений (включая `[Flags]`) и конвертация "на лету" через методы расширения (`.AsTag()`, `.AsEnum()`).
* **Битовые операции:** Перегруженные операторы (`|`, `&`, `^`, `~`) для интуитивной работы с `FlagsHandle`.
* **Сетевая поддержка:** Пакет предусматривает обертки `NetTagHandle` и `NetFlagsHandle` для сериализации по сети.

---

## 📁 Структура и логика работы

1. **`TagsSource` (ScriptableObject):** Базовый источник тегов. Содержит массив из 32 строк. Кэширует их в `FrozenDictionary` для быстрого поиска по имени.
2. **`TagSourceEnum` (ScriptableObject):** Наследник `TagsSource`. Принимает имя типа C# Enum и автоматически заполняет массив из 32 тегов на основе его значений (поддерживает обычные enum и `[Flags]`).
3. **`TagsStore`:** Словарь-коллекция, привязывающая конкретные `TagsSource` к `FlagsHandle`. Используется внутри `TagsComponent`.
4. **`TagHandle` & `FlagsHandle`:** Непосредственно сами теги. Легковесные структуры. `FlagsHandle` содержит битовую маску, позволяющую хранить комбинацию тегов из одного источника.

---

## ⚙️ Настройка и создание тегов (Setup)

Система требует создания источников тегов (`TagsSource` или `TagSourceEnum`) в проекте.

### Способ 1. Обычный TagsSource (через строки)
1. Создайте ассет: `Create -> FrozenBox -> TagsSource`.
2. Заполните массив строковыми названиями тегов в инспекторе. 

### Способ 2. Интеграция C# Enum (TagSourceEnum)
Это рекомендуемый способ для тегов, завязанных на логику (например, `DamageType`, `PlayerState`).

1. Создайте обычный `enum` в скрипте:
   
```csharp
   public enum DamageType
   {
       Physical,
       Magic,
       TrueDamage
   }
   
   [Flags]
   public enum StatusEffects
   {
       Stunned = 1 << 0,
       Poisoned = 1 << 1,
       Burning = 1 << 2
   }
```

## 🧠 Глубокое погружение: TagHandle, FlagsHandle и TagsStore

Архитектура построена на трех китах: одиночном теге, наборе тегов одного типа и глобальном хранилище разнотипных тегов.

### 1. `TagHandle` (Одиночный тег)

`TagHandle` — это легковесная структура, представляющая один конкретный тег. Она хранит ссылку на свой источник (`TagsSource`) и свой порядковый индекс (`Index` от 0 до 31).
Система автоматически вычисляет его битовый флаг (`1 << _index`).

**Пример работы:**

```csharp
using FrozenBox.TagsSystem;
using UnityEngine;

public class TargetType : MonoBehaviour
{
    // Получаем TagHandle через EnumSupport (extension метод)
    public TagHandle GetTargetTag() => EntityType.Player.AsTag().Value;

    public void CompareTags(TagHandle otherTag)
    {
        // Сравнение тегов происходит мгновенно, без аллокаций
        if (this.GetTargetTag() == otherTag) 
        {
            Debug.Log("Теги совпадают!");
        }
    }
}

```

### 2. `FlagsHandle` (Набор тегов одного источника)

`FlagsHandle` — это структура, хранящая битовую маску (`int _flags`) тегов, принадлежащих **к одному и тому же `TagsSource**`.
Она перегружает побитовые операторы (`|`, `&`, `^`, `~`) для интуитивной работы. Все операторы защищены `Assert.AreEqual`, что означает: *вы не можете случайно сложить тег урона с тегом состояния — система выдаст ошибку*.

**Пример работы:**

```csharp
using FrozenBox.TagsSystem;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private FlagsHandle _currentStates;

    public void Init(TagsSource stateSource)
    {
        // Инициализация пустой маски, привязанной к конкретному источнику
        _currentStates = FlagsHandle.EmptyOf(stateSource);
    }

    public void AddState(TagHandle stateTag)
    {
        // Добавление тега (побитовое ИЛИ)
        _currentStates |= stateTag;
    }

    public void RemoveState(TagHandle stateTag)
    {
        // Удаление тега (побитовое И & побитовое НЕ)
        _currentStates &= ~stateTag;
    }

    public void ToggleState(TagHandle stateTag)
    {
        // Переключение состояния тега (Исключающее ИЛИ)
        _currentStates ^= stateTag;
    }

    public void CheckStates(TagHandle stunTag, FlagsHandle negativeEffects)
    {
        // Проверка наличия одного тега
        bool isStunned = _currentStates.HasTag(stunTag);

        // Проверка пересечения с другой маской (есть ли ХОТЯ БЫ ОДИН общий тег)
        bool hasAnyNegative = _currentStates.HasAny(negativeEffects);

        // Проверка вхождения маски (есть ли ВСЕ теги из переданной маски)
        bool hasAllNegative = _currentStates.HasAll(negativeEffects);
        
        // Проверка, есть ли вообще хоть какие-то теги
        bool hasAnything = _currentStates.HasAny(); 
    }
}

```

### 3. `TagsStore` (Глобальное хранилище сущности)

Объект в игре часто должен иметь теги из *разных* источников (например, он одновременно принадлежит к фракции "Орки", имеет тип брони "Тяжелая" и состояние "Отравлен").
`TagsStore` решает эту задачу. Это сериализуемый словарь (`SerializableDictionary`), где ключом выступает `TagsSource`, а значением — `FlagsHandle` (набор тегов этого источника).

`TagsStore` обычно используется как часть `TagsComponent`, который вешается на GameObject.

**Пример работы:**

```csharp
using FrozenBox.TagsSystem;
using UnityEngine;

[RequireComponent(typeof(TagsComponent))]
public class DamageReceiver : MonoBehaviour
{
    private TagsStore _entityTags;

    private void Awake()
    {
        // TagsComponent предоставляет доступ к TagsStore
        _entityTags = GetComponent<TagsComponent>().TagsStore;
    }

    public void TakeDamage(int amount, TagHandle damageTypeTag)
    {
        // TagsStore сам определяет, к какому TagsSource относится переданный тег,
        // и проверяет соответствующий FlagsHandle внутри себя.
        if (_entityTags.HasTag(damageTypeTag))
        {
            Debug.Log("Урон заблокирован: сущность имеет иммунитет (тег) к этому типу урона!");
            return;
        }

        // ... применение урона
    }
    
    public void CheckMultipleConditions(FlagsHandle requiredTags)
    {
        // Можно передать целую маску, и TagsStore проверит наличие всех тегов (HasAll) 
        // или хотя бы одного (HasAny) в соответствующем источнике сущности.
        if (_entityTags.HasAll(requiredTags))
        {
            Debug.Log("У сущности есть все необходимые теги из запрошенной маски.");
        }
    }
}

```

---

## 🛠️ Вспомогательные классы

* **`TaggedLibrary<T>`:** Базовый класс для создания библиотек, связывающих какие-либо элементы с их `TagsStore`.
* **`TagsGroupsLibrary`:** Библиотека ассетов-источников, позволяющая удобно группировать и загружать `TagsSource`.
