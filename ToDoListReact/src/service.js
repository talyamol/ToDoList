import axios from 'axios';

// = "https://localhost:7271";
axios.defaults.baseURL = 'http://localhost:5271'; 

axios.interceptors.response.use(
  response => response,  // אם התשובה היא תקינה, חזורי עליה כמו שהיא
  error => {
      console.error('HTTP Request Error:', error);  // רשום את השגיאה ללוג
      return Promise.reject(error);  //  .החזרי את השגיאה כדי שתטופל באופן נוסף או תוכלי להציגה למשתמש
  }
);


export default {
    getTasks: async () => {
        const result = await axios.get(`/tasks`);
        return result.data;
    },

    addTask: async (name) => {
        const result = await axios.post(`/tasks`, { name });
        return result.data;
    },

    setCompleted: async (id, isComplete) => {
        const result = await axios.put(`/tasks/${id}`, { isComplete });
        return result.data;
    },

    deleteTask: async (id) => {
        await axios.delete(`/tasks/${id}`);
    }
};
