(function () {
    function startObserver() {
        if (!document.body) {
            document.addEventListener('DOMContentLoaded', startObserver);
            return;
        }

        try {
            const observer = new MutationObserver((_mutations, obs) => {
                const authWrapper = document.querySelector('.auth-wrapper');
                if (authWrapper) {
                    authWrapper.addEventListener('click', function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        showLoginModal();
                    });

                    if (localStorage.getItem('accessToken')) {
                        setSwaggerToken(localStorage.getItem('accessToken'));
                    }

                    obs.disconnect();
                }
            });

            observer.observe(document.body, {
                childList: true,
                subtree: true,
                attributes: false,
                characterData: false
            });
        } catch (error) {
            console.error('启动 MutationObserver 时出错:', error);
        }
    }

    function showLoginModal() {
        const modal = document.createElement('div');
        modal.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 10000;
        `;

        const modalContent = document.createElement('div');
        modalContent.style.cssText = `
            background-color: white;
            padding: 30px;
            border-radius: 8px;
            width: 400px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        `;

        modalContent.innerHTML = `
            <h2 style="margin-top: 0; color: #333;">登录</h2>
            <form id="loginForm" style="margin-top: 20px;">
                <div style="margin-bottom: 15px;">
                    <label style="display: block; margin-bottom: 5px; font-weight: 500;">邮箱</label>
                    <input type="email" id="email" value="admin@example.com" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px; box-sizing: border-box;">
                </div>
                <div style="margin-bottom: 20px;">
                    <label style="display: block; margin-bottom: 5px; font-weight: 500;">密码</label>
                    <input type="password" id="password" value="Admin123!" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px; box-sizing: border-box;">
                </div>
                <div style="display: flex; gap: 10px; justify-content: flex-end;">
                    <button type="button" id="cancelBtn" style="padding: 8px 16px; border: 1px solid #ddd; border-radius: 4px; background-color: #f5f5f5; cursor: pointer;">取消</button>
                    <button type="submit" style="padding: 8px 16px; border: 1px solid #409eff; border-radius: 4px; background-color: #409eff; color: white; cursor: pointer;">登录</button>
                </div>
            </form>
        `;

        modal.appendChild(modalContent);
        document.body.appendChild(modal);

        document.getElementById('cancelBtn').addEventListener('click', function () {
            document.body.removeChild(modal);
        });

        document.getElementById('loginForm').addEventListener('submit', function (e) {
            e.preventDefault();
            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;

            // 调用登录接口
            login(email, password).then(token => {
                if (token) {
                    setSwaggerToken(token);
                    document.body.removeChild(modal);
                }
            }).catch(error => {
                console.error('登录失败:', error);
            });
        });
    }

    async function login(email, password) {
        const response = await fetch('/api/v1/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ Email: email, Password: password, RememberMe: false })
        });
        if (!response.ok) {
            throw new Error('登录失败');
        }
        const data = await response.json();
        const token = data.data.tokenType + ' ' + data.data.accessToken;
        localStorage.setItem('accessToken', token);
        return token;
    }

    function setSwaggerToken(token) {
        // 尝试直接修改swagger的配置，添加认证头
        if (window.ui && window.ui.getConfigs) {
            const configs = window.ui.getConfigs();
            if (configs.requestInterceptor) {
                const originalInterceptor = configs.requestInterceptor;
                configs.requestInterceptor = (request) => {
                    request.headers['Authorization'] = token;
                    return originalInterceptor(request);
                };
            } else {
                configs.requestInterceptor = (request) => {
                    request.headers['Authorization'] = token;
                    return request;
                };
            }
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', startObserver);
    } else {
        startObserver();
    }
})();
