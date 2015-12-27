# Collaborative filtering

This pack of programs/libs can help you with **Collaborative filtering algorithm** in your app.
Next description taken in Russian


## Пакет программ/библиотек
- CollaborativeFiltering - библиотека для выполнения user-based или item-based коллабаративной фильтрации
- CollaborativeFilteringConsole - консоль, для использования библиотеки
- CollaborativeAgent - консоль - обработчик, подключающийся к серверу (пока не реализовано) или загружающий тестовые данные из файла и проверяющий их корректность
- CollaborativeFilteringTest - unit-тесты для библиотеки
- DatasetSplitter - консольное приложение для разбиения данных на чанки
- KPILibrary - заимствованная библиотека для работы с Smart M3
- csharp-generator.rb - генератор кода тест кейса из csv файла

### CollaborativeFiltering
Библиотека, для осуществления предсказаний оценок.

**Пример использования**

```
// создадим анализатор
var analyzer = new CollaborativeFiltering.Analyzer();
// добавим оценки
analyzer.AddMark(new CollaborativeFiltering.Mark(1, 1, 2));
analyzer.AddMark(new CollaborativeFiltering.Mark(2, 1, 4));
analyzer.AddMark(new CollaborativeFiltering.Mark(4, 1, 5));
analyzer.AddMark(new CollaborativeFiltering.Mark(7, 1, 3));
analyzer.AddMark(new CollaborativeFiltering.Mark(9, 1, 2));
// ... еще оценки

// рассчитаем коэффициенты
analyzer.InitCoefficients();

// получим матрицу оценок (в которой оценки со знаком "-" - исходные, "+" - рассчитанные)
var matrix = analyzer.GetMarks(CollaborativeFiltering.BaseAnalyzer.FilteringType.UserBased);

// проверим, что для пользователя 1 предсказана оценка 3 для объекта 5
Assert.AreEqual(3, matrix[1][5]);
```

### CollaborativeAgent
Консоль - обработчик, подключающийся к серверу (пока не реализовано) или загружающий тестовые данные из файла и проверяющий их корректность.

#### Входные параметры
* **путь к индекс файлу** [обязательный] 

#### Формат индекс файла
Индекс файл представляет из себя csv файл без заголовка, с колонками **variant** и **answer_file**.
Answer-файл - файл ответов, полученный из CollaborativeFilteringConsole

##### Пример индекс файла

```
1,.\AgentData\1-answer.csv
```




### CollaborativeFilteringConsole
Консоль, рассчитывающая на основе файла с существующими оценками, предсказанные оценки.

#### Входные параметры
* **путь к исходному файлу** [обязательный] 
* **режим рассчета - item или user** [обязательный] 
* файл вывода рассчитанных оценок [необязательный]

#### Формат файлов
Файлы должны быть написаны, используя формат CSV, но **без заголовков**.

Колонки должны идти в следующей последовательности
* user
* item
* rate



## Used libraries
- [M3 C# KPI](http://sourceforge.net/projects/m3-csharp-kpi/)


