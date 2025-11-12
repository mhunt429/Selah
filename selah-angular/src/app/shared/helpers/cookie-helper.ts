export const getCookieValue = (cookieName: string) => {
  return (
    document.cookie
      .split('; ')
      .map((cookie) => cookie.split('='))
      .find(([key]) => key === cookieName)?.[1] || ''
  );
};
