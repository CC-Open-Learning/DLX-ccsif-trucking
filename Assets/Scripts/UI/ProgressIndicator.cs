using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Class that allows for progress indicator V1 functionality in UI.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ProgressIndicator : MonoBehaviour, IUserInterface
    {
        [Header("Progress Indicator Settings")]
        [SerializeField]
        private VisualTreeAsset categoryAsset;

        [SerializeField]
        private VisualTreeAsset taskAsset;

        [SerializeField]
        private List<Category> categories;

        private Dictionary<Category, VisualElement> categoryElements = new Dictionary<Category, VisualElement>();
        private Dictionary<Category, CategoryUI> categoryUIs = new Dictionary<Category, CategoryUI>();
        private Dictionary<Task, TaskUI> taskUIs = new Dictionary<Task, TaskUI>();

        /// <summary>
        /// The root visual element of the progress indicator.
        /// </summary>
        public VisualElement Root { get; set; }

        //private VisualElement progressIndicator;
        private VisualElement categoriesHolder;

        private const string ArrowClosedClass = "progress-v1-category-arrow-closed";
        private const string CategoryLastBorderRadiusClass = "progress-v1-category-last";
        private const string CategoryLastBottomClass = "display-hidden";
        private const string TaskCompletedLabelClass = "progress-v1-task-completed-label";

        /// <summary>
        /// Invokes when the progress indicator is set to visible.
        /// </summary>
        [Header("Events"), Space(4f)]
        [Tooltip("Invokes when the progress indicator is set to visible.")]
        public UnityEvent OnShow;

        /// <summary>
        /// Invokes when the progress indicator is set to hidden.
        /// </summary>
        [Tooltip("Invokes when the progress indicator is set to hidden.")]
        public UnityEvent OnHide;

        /// <summary>
        /// Invokes when the progress of a task has been updated. Event paramters: The index of category containing the updated task, The index of the updated task.
        /// </summary>
        [Tooltip("Invokes when the progress of a task has been updated. Event paramters: The index of category containing the updated task, The index of the updated task.")]
        public UnityEvent<int, int> OnProgressUpdated;

        /// <summary>
        /// Invokes when a new category has been added to the progress indicator. Event parameter: The index of the newly created category.
        /// </summary>
        [Tooltip("Invokes when a new category has been added to the progress indicator. Event parameter: The index of the newly created category.")]
        public UnityEvent<int> OnCategoryAdded;

        /// <summary>
        /// Invokes when a new task has been added to a category. Event parameters: The index of the category the task has been added to, The index of the newly created task.
        /// </summary>
        [Tooltip("Invokes when a new task has been added to a category. Event parameters: The index of the category the task has been added to, The index of the newly created task.")]
        public UnityEvent<int, int> OnTaskAdded;

        /// <summary>
        /// Invokes when a category has been removed from the progress indicator.
        /// </summary>
        [Tooltip("Invokes when a category has been removed from the progress indicator.")]
        public UnityEvent OnCategoryRemoved;

        /// <summary>
        /// Invokes when a task has been removed from a category. Event parameter: The index of the category the task was removed from.
        /// </summary>
        [Tooltip("Invokes when a task has been removed from a category. Event parameter: The index of the category the task was removed from.")]
        public UnityEvent<int> OnTaskRemoved;

        /// <summary>
        /// Invokes when the arrow category is expanded or contrasted.
        /// </summary>
        [Tooltip("Invokes when the arrow category is expanded or contrasted.")]
        public UnityEvent OnArrowClicked;

        /// <summary>
        /// The number of categories in the progress indicator.
        /// </summary>
        public int CategoryCount
        {
            get => categories.Count;
        }

        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();
            Root = document.rootVisualElement;
            //progressIndicator = Root.Q("ProgressIndicator");
            categoriesHolder = Root.Q("CategoryHolder");

            categories ??= new List<Category>();
            OnShow ??= new UnityEvent();
            OnHide ??= new UnityEvent();
            OnProgressUpdated ??= new UnityEvent<int, int>();
            OnCategoryAdded ??= new UnityEvent<int>();
            OnCategoryRemoved ??= new UnityEvent();
            OnTaskAdded ??= new UnityEvent<int,int>();
            OnTaskRemoved ??= new UnityEvent<int>();
            OnArrowClicked ??= new UnityEvent();

            OnCategoryAdded.AddListener(OnCategoryAddedToList);
            OnCategoryRemoved.AddListener(OnCategoryRemoveFromList);

            GenerateCategories();

            HandleDisplayUI();
        }

        /// <summary>
        /// Displays the progress indicator with all its categories and tasks.
        /// </summary>
        public void HandleDisplayUI()
        {
            Show();
        }

        // Generate the categories already initialized in the categories list.
        private void GenerateCategories()
        {
            foreach (Category category in categories)
            {
                GenerateCategory(category);

                foreach (Task task in category.Tasks)
                {
                    GenerateTask(category, task);
                }
            }
        }

        // Internal method for generating the UI part of the category.
        private void GenerateCategory(Category category)
        {
            VisualElement newCategory = categoryAsset.CloneTree();
            newCategory.Q("Category").EnableInClassList(CategoryLastBorderRadiusClass, false);
            newCategory.Q("Bottom").EnableInClassList(CategoryLastBottomClass, false);
            newCategory.Q<Label>("TitleLabel").text = category.Name;



            VisualElement taskHolder = newCategory.Q("TaskHolder");
            categoriesHolder.Add(newCategory);

            // Arrow
            VisualElement arrowHolder = newCategory.Q("ArrowHolder");
            VisualElement arrow = newCategory.Q("Arrow");
            arrow.EnableInClassList(ArrowClosedClass, false);
            arrowHolder.RegisterCallback<ClickEvent, VisualElement>(ArrowAnimation, arrow);
            arrowHolder.RegisterCallback<ClickEvent, VisualElement>(ToggleTaskView, taskHolder);
            arrowHolder.RegisterCallback<ClickEvent>(OnArrowHolderClicked);

            // Progress Bar and Progress Label
            int taskCount = category.Tasks.Count;

            // UI
            CategoryUI categoryUI = new CategoryUI()
            {
                TaskHolder = taskHolder,
            };

            // Add to lists
            categoryElements.Add(category, newCategory);
            categoryUIs.Add(category, categoryUI);
        }

        //event for arrow holder being clicked
        private void OnArrowHolderClicked(ClickEvent evt)
        {
            OnArrowClicked?.Invoke();
        }

        // Internal method for generating the UI part of the task.
        private void GenerateTask(Category category, Task task)
        {
            VisualElement newTask = taskAsset.CloneTree();

            // Task name label
            Label taskNameLabel = newTask.Q<Label>("TaskLabel");
            taskNameLabel.text = task.Name;
            taskNameLabel.EnableInClassList(TaskCompletedLabelClass, false);

            // Task checkmark
            VisualElement iconContainer = newTask.Q("IconContainer");
            //iconContainer.EnableInClassList(TaskCheckmarkCheckedClass, false);
            VisualElement checkmarkIcon = iconContainer.Q("Checkmark");
            VisualElement crossIcon = iconContainer.Q("Cross");
            
            Label scoreLabel = newTask.Q<Label>("ScoreLabel");

            TaskUI taskUI = new TaskUI();
            
            if (scoreLabel != null)
            {
                taskUI = new()
                { 
                    TaskNameLabel = taskNameLabel,
                    //IconContainer = iconContainer,
                    CheckmarkIcon = checkmarkIcon,
                    CrossIcon = crossIcon,
                    PointIterationLabel = scoreLabel
                };  
            }
            else
            { 
                taskUI = new()
                { 
                    TaskNameLabel = taskNameLabel,
                    //IconContainer = iconContainer,
                    CheckmarkIcon = checkmarkIcon,
                    CrossIcon = crossIcon
                };  
            }
            
            

            taskUI.CheckmarkIcon.style.display = DisplayStyle.None;
            taskUI.CrossIcon.style.display = DisplayStyle.Flex;

            categoryUIs[category].TaskHolder.Add(newTask);
            taskUIs.Add(task, taskUI);
        }

        // Arrow animation
        private void ArrowAnimation(ClickEvent e, VisualElement arrow)
        {
            arrow.ToggleInClassList(ArrowClosedClass);
        }

        // Arrow functionality
        private void ToggleTaskView(ClickEvent e, VisualElement taskHolder)
        {
            if (taskHolder.style.display == DisplayStyle.None)
            {
                UIHelper.Show(taskHolder);
            }
            else
            {
                UIHelper.Hide(taskHolder);
            }
        }

        /// <summary>
        /// Adds a new category to the progress indicator.
        /// </summary>
        /// <param name="name">The name of the new category.</param>
        public void AddCategory(string name)
        {
            Category category = new Category(name);

            categories.Add(category);
            GenerateCategory(category);
            OnCategoryAdded.Invoke(CategoryCount - 1);
        }

        /// <summary>
        /// Adds a new task to a specific category in the progress indicator.
        /// </summary>
        /// <param name="categoryIndex">The index of the category to add the task to.</param>
        /// <param name="taskName">The name of the new task.</param>
        public void AddTask(int categoryIndex, string taskName)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to add task at invalid category index {categoryIndex}");
                return;
            }

            Task task = new Task(taskName);

            category.Tasks.Add(task);
            GenerateTask(categories[categoryIndex], task);
            UpdateUI(categories[categoryIndex], task);
            OnTaskAdded.Invoke(categoryIndex, GetTaskCount(categoryIndex) - 1);
        }

        public void AddTask(int categoryIndex, string taskName, string points)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to add task at invalid category index {categoryIndex}");
                return;
            }

            Task task = new Task(taskName, points);

            category.Tasks.Add(task);
            GenerateTask(categories[categoryIndex], task);
            UpdateUI(categories[categoryIndex], task);
            OnTaskAdded.Invoke(categoryIndex, GetTaskCount(categoryIndex) - 1);
        }

        /// <summary>
        /// Removes a task at the specified category and task indices.
        /// </summary>
        /// <param name="categoryIndex">The category index of the task.</param>
        /// <param name="taskIndex">The index of the task to remove.</param>
        public void RemoveTask(int categoryIndex, int taskIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to remove task at invalid category index {categoryIndex}");
                return;
            }

            Task task = category.Tasks.ElementAtOrDefault(taskIndex);
            if (task == null)
            {
                Debug.LogError($"Unable to remove task at invalid task index {taskIndex}");
                return;
            }

            taskUIs.Remove(category.Tasks[taskIndex]);
            category.Tasks.RemoveAt(taskIndex);
            categoryUIs[category].TaskHolder.RemoveAt(taskIndex);
            UpdateCategoryUI(category);
            OnTaskRemoved.Invoke(categoryIndex);
        }

        /// <summary>
        /// Removes a category at the specified index.
        /// </summary>
        /// <param name="categoryIndex">The index of the category to remove.</param>
        public void RemoveCategory(int categoryIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to remove category at invalid category index {categoryIndex}");
                return;
            }

            for (int i = 0; i < GetTaskCount(categoryIndex); i++)
            {
                RemoveTask(categoryIndex, i);
            }

            categoryUIs.Remove(category);
            categoryElements.Remove(category);
            categoriesHolder.RemoveAt(categoryIndex);
            categories.RemoveAt(categoryIndex);
            OnCategoryRemoved.Invoke();
        }

        /// <summary>
        /// Adds progress to a specific task in the progress indicator.
        /// </summary>
        /// <param name="categoryIndex">The category containing the task.</param>
        /// <param name="taskIndex">The task to add progress to.</param>
        /// <param name="amount">The amount of progress to add to the task's progression (default value is 1).</param>
        public void AddProgressToTask(int categoryIndex, int taskIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to add progress to task at invalid category index {categoryIndex}");
                return;
            }

            Task task = category.Tasks.ElementAtOrDefault(taskIndex);
            if (task == null)
            {
                Debug.LogError($"Unable to add progress to task at invalid task index {taskIndex}");
                return;
            }

            task.Completed = true;
            OnProgressUpdated.Invoke(categoryIndex, taskIndex);

            UpdateUI(categories[categoryIndex], task);
        }

        // Internal method for updating category UI
        private void UpdateCategoryUI(Category category)
        {
            int tasksComplete = 0;
            foreach (Task t in category.Tasks)
            {
                if (t.Completed)
                {
                    tasksComplete++;
                }
            }

            CategoryUI categoryUI = categoryUIs[category];
        }

        // Internal method for updating both category and task UI when tasks have been updated (new task added, progress added, etc).
        private void UpdateUI(Category category, Task task)
        {
            TaskUI taskUI = taskUIs[task];

            if (task.Completed)
            {
                //taskUI.TaskNameLabel.EnableInClassList(TaskCompletedLabelClass, true);
                //taskUI.IconContainer.EnableInClassList(TaskCheckmarkCheckedClass, true);
                taskUI.CheckmarkIcon.style.display = DisplayStyle.Flex;
                taskUI.CrossIcon.style.display = DisplayStyle.None;
                if (task.Score != null)
                {
                    taskUI.PointIterationLabel.text = "+" + task.Score + " Points";
                }
            }
            else
            {
                if (task.Score != null)
                {
                    taskUI.PointIterationLabel.text = "-" + task.Score + " Points";
                }
            }

            UpdateCategoryUI(category);
        }

        // This function is for the round borders of the bottom element
        private void SetBottomCategory(VisualElement category, bool bottom)
        {
            VisualElement background = category.Q("Category");
            VisualElement bottomElement = category.Q("Bottom");
            VisualElement arrowHolder = category.Q("ArrowHolder");

            if (bottom)
            {
                arrowHolder.RegisterCallback<ClickEvent, (VisualElement, VisualElement)>(ToggleBorderClass, (background, bottomElement));
            }
            else
            {
                arrowHolder.UnregisterCallback<ClickEvent, (VisualElement, VisualElement)>(ToggleBorderClass);
            }
        }

        // Toggles the rounded corners on the last category element
        private void ToggleBorderClass(ClickEvent e, (VisualElement, VisualElement) elements)
        {
            elements.Item1.ToggleInClassList(CategoryLastBorderRadiusClass);
            elements.Item2.ToggleInClassList(CategoryLastBottomClass);
        }

        /// <summary>
        /// Shows the progress indicator.
        /// </summary>
        public void Show()
        {
            UIHelper.Show(Root);
            OnShow.Invoke();
        }

        /// <summary>
        /// Hides the progress indicator.
        /// </summary>
        public void Hide()
        {
            UIHelper.Hide(Root);
            OnHide.Invoke();
        }

        /// <summary>
        /// Returns the amount of tasks in a category.
        /// </summary>
        /// <param name="categoryIndex">The index of the category to check.</param>
        /// <returns>The amount of tasks in the specified category.</returns>
        public int GetTaskCount(int categoryIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to get task at invalid category index {categoryIndex}");
                return -1;
            }

            return category.Tasks.Count;
        }

        // Add round corners to the last category in the list
        private void AddBottomClassesToLastCategory(int index)
        {
            if (index > 0)
            {
                SetBottomCategory(categoryElements[categories[index - 1]], false);
            }
            SetBottomCategory(categoryElements[categories[index]], true);
        }

        private void OnCategoryAddedToList(int index)
        {
            AddBottomClassesToLastCategory(index);
        }

        private void OnCategoryRemoveFromList()
        {
            if (CategoryCount > 0)
            {
                AddBottomClassesToLastCategory(CategoryCount - 1);
            }
        }

        // Cleanup
        private void OnDestroy()
        {
            OnCategoryAdded?.RemoveListener(OnCategoryAddedToList);
            OnCategoryRemoved?.RemoveListener(OnCategoryRemoveFromList);
        }
    }
}
