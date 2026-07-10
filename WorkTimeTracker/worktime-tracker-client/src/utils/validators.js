export const validators = {
  required: (value) => {
    if (!value && value !== 0) return 'Поле обязательно для заполнения';
    if (typeof value === 'string' && !value.trim()) return 'Поле обязательно для заполнения';
    return null;
  },
  
  maxLength: (max) => (value) => {
    if (value && value.length > max) {
      return `Максимальная длина ${max} символов`;
    }
    return null;
  },
  
  range: (min, max) => (value) => {
    if (value !== undefined && value !== null) {
      if (value < min) return `Минимальное значение ${min}`;
      if (value > max) return `Максимальное значение ${max}`;
    }
    return null;
  },
  
  positive: (value) => {
    if (value !== undefined && value !== null && value <= 0) {
      return 'Значение должно быть положительным';
    }
    return null;
  },
  
  date: (value) => {
    if (value) {
      const date = new Date(value);
      if (isNaN(date.getTime())) {
        return 'Неверный формат даты';
      }
    }
    return null;
  },
};