FROM centos:7

# ENV LANG=en_US.UTF-8

ARG USER_ID
ARG GROUP_ID

#RUN sed -i s@/archive.ubuntu.com/@/mirrors.aliyun.com/@g /etc/apt/sources.list
#RUN sed -i s@/security.ubuntu.com/@/mirrors.aliyun.com/@g /etc/apt/sources.list

RUN yum clean all
RUN yum -y install epel-release
RUN yum -y groupinstall "Development Tools"
RUN yum -y install wget perl-CPAN gettext-devel perl-devel  openssl-devel  zlib-devel curl-devel expat-devel  getopt asciidoc xmlto docbook2X
RUN ln -s /usr/bin/db2x_docbook2texi /usr/bin/docbook2x-texi

RUN yum -y install openssh-server initscripts
RUN sshd-keygen

# Enable root and bxu accounts
RUN echo 'root:root123' | chpasswd
RUN groupadd -g $GROUP_ID dev
# add user
RUN useradd -m -s /bin/bash --uid $USER_ID --gid $GROUP_ID bxu
RUN echo "bxu:123456" | chpasswd
RUN chown -R bxu:dev /home/bxu
	
# Configure SSHD
RUN mkdir -p /var/run/sshd ; chmod -rx /var/run/sshd
# http://stackoverflow.com/questions/2419412/ssh-connection-stop-at-debug1-ssh2-msg-kexinit-sent
# RUN ssh-keygen -t rsa -f /etc/ssh/ssh_host_rsa_key
# Bad security, add a user and sudo instead!
RUN sed -ri 's/#PermitRootLogin yes/PermitRootLogin yes/g' /etc/ssh/sshd_config
# http://stackoverflow.com/questions/18173889/cannot-access-centos-sshd-on-docker
RUN sed -ri 's/UsePAM yes/#UsePAM yes/g' /etc/ssh/sshd_config
RUN sed -ri 's/#UsePAM no/UsePAM no/g' /etc/ssh/sshd_config

# Deploy ssh keys
# RUN mkdir /root/.ssh/ && \
# 	echo "ssh-rsa AAAAB3Nz4........l9Ns5p989oHLcSGJ" > ~/.ssh/authorized_keys && \
# 	chmod 700 ~/.ssh && \
# 	chmod 600 ~/.ssh/authorized_keys

# RUN mkdir /home/pos/.ssh/ && \
# 	echo "ssh-rsa AAAAB3NzaC........9Ns5p989oHLcSGJ" > /home/pos/.ssh/authorized_keys && \
# 	chmod 700 /home/pos/.ssh && \
# 	chmod 600 /home/pos/.ssh/authorized_keys && \
# 	chown -R pos:pos /home/pos/.ssh/

EXPOSE 22

CMD ["/usr/sbin/sshd", "-D"]